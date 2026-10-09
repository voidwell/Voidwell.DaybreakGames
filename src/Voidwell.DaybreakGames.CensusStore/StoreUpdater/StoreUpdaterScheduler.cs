using System.Reflection;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Data;
using Voidwell.DaybreakGames.Data.Extensions;
using Voidwell.DaybreakGames.Data.Models;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;
using Voidwell.DaybreakGames.Utils.HostedService;

namespace Voidwell.DaybreakGames.CensusStore.StoreUpdater;

public class StoreUpdaterScheduler : IStoreUpdaterService, IStatefulHostedService
{
    private readonly IUpdaterSchedulerRepository _updaterSchedulerRepository;
    private readonly IDbContextFactory<PS2DbContext> _dbContextFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly StaticStoreUpdaterConfiguration _options;
    private readonly ILogger<StoreUpdaterScheduler> _logger;
    private readonly IMapper _mapper;

    private readonly Dictionary<Type, LastStoreUpdate> _collectionLog = new();
    private readonly Dictionary<Type, Timer> _updaterTimers = new();

    private readonly SemaphoreSlim _updateLock = new(1);

    public StoreUpdaterScheduler(IUpdaterSchedulerRepository updaterSchedulerRepository, IDbContextFactory<PS2DbContext> dbContextFactory,
        IServiceProvider serviceProvider, IOptions<StaticStoreUpdaterConfiguration> options, ILogger<StoreUpdaterScheduler> logger,
        IMapper mapper)
    {
        _updaterSchedulerRepository = updaterSchedulerRepository;
        _dbContextFactory = dbContextFactory;
        _serviceProvider = serviceProvider;
        _options = options.Value;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task OnApplicationStartup(CancellationToken cancellationToken)
    {
        foreach (var updateConfig in _options.Collections)
        {
            var updaterHistory = await _updaterSchedulerRepository.GetUpdaterHistoryByServiceNameAsync(updateConfig.StoreName!);

            UpdateServiceLog(updateConfig, updaterHistory?.LastUpdateDate);
        }
    }

    public Task OnApplicationShutdown(CancellationToken cancellationToken)
    {
        _updaterTimers.Clear();

        return Task.CompletedTask;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_options.Enabled)
        {
            return Task.CompletedTask;
        }

        foreach (var collectionType in _collectionLog.Keys)
        {
            RegisterUpdaterTimer(collectionType);
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _updaterTimers.ToList().ForEach(a => a.Value?.Dispose());
        _updaterTimers.Clear();

        return Task.CompletedTask;
    }

    public IEnumerable<LastStoreUpdate> GetStoreUpdateLog()
    {
        return _collectionLog.Values.OrderBy(a => a.StoreName).ToList();
    }

    public Task<object> GetStatusAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<object>($"Tracking {_collectionLog.Count} updatable collections");
    }

    public async Task<LastStoreUpdate?> UpdateStoreAsync(string storeName)
    {
        var updateConfig = _options?.Collections?.FirstOrDefault(a => a.StoreName == storeName);
        if (updateConfig == null)
        {
            return null;
        }

        await RunStoreUpdateAsync(updateConfig.CollectionType!);

        return _collectionLog[updateConfig.CollectionType!];
    }

    private async Task<bool> RunStoreUpdateAsync(Type collectionType)
    {
        var updateConfig = _options.Collections.First(a => a.CollectionType == collectionType);

        if (updateConfig.Dependencies != null)
        {
            foreach (var dependency in updateConfig.Dependencies)
            {
                if (!await RunStoreUpdateAsync(dependency))
                {
                    return false;
                }
            }
        }

        await _updateLock.WaitAsync();

        try
        {
            if (_collectionLog[collectionType].LastUpdated != null && DateTime.UtcNow - _collectionLog[collectionType].LastUpdated < TimeSpan.FromSeconds(60))
            {
                return true;
            }

            _logger.LogInformation("Updating {StoreName}.", updateConfig.StoreName);

            var collectionValues = await GetCollectionValuesAsync(updateConfig.CollectionType!);
            if (collectionValues == null)
            {
                return false;
            }

            var entityValues = MapEntityValues(updateConfig.EntityType!, collectionValues);

            await UpsertAsync(entityValues);

            _logger.LogInformation("Update complete for {StoreName}.", updateConfig.StoreName);

            var dataModel = new UpdaterScheduler
            {
                Id = updateConfig.StoreName,
                LastUpdateDate = DateTime.UtcNow
            };
            await _updaterSchedulerRepository.UpsertAsync(dataModel);

            UpdateServiceLog(updateConfig, dataModel.LastUpdateDate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Update failed for {StoreName}", updateConfig.StoreName);
            return false;
        }
        finally
        {
            _updateLock.Release();
        }

        return true;
    }

    private readonly MethodInfo _upsertMethod = typeof(DbSetExtensions).GetMethods()
                .First(a => a.Name == "UpsertAsync" && a.GetParameters()[1].ParameterType.IsGenericType && typeof(IEnumerable<>).IsAssignableTo(a.GetParameters()[1].ParameterType.GetGenericTypeDefinition()));
    private async Task UpsertAsync(object values)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        var task = (Task)_upsertMethod.MakeGenericMethod(values.GetType().GetGenericArguments()[0])
            .Invoke(dbContext, new[] { dbContext, values })!;

        await task!.WaitAsync(CancellationToken.None);
    }

    private object MapEntityValues(Type entityType, object values)
    {
        var valueType = values.GetType().BaseType!.GetGenericArguments()[0];
        var valueTypeList = typeof(IEnumerable<>).MakeGenericType(valueType);
        var entityTypeList = typeof(IEnumerable<>).MakeGenericType(entityType);

        return _mapper.Map(values, valueTypeList, entityTypeList);
    }

    private async Task<object?> GetCollectionValuesAsync(Type staticCollectionType)
    {
        var updaterService = _serviceProvider.GetRequiredService(staticCollectionType);

        var methodInfo = updaterService.GetType().GetMethod(nameof(ICensusStaticCollection<object>.GetCollectionAsync));
        var task = (Task)methodInfo!.Invoke(updaterService, null)!;

        await task!.WaitAsync(CancellationToken.None);

        var result = task.GetType().GetProperty(nameof(Task<object>.Result))!.GetValue(task);

        return result;
    }

    private void RegisterUpdaterTimer(Type collectionType)
    {
        if (_updaterTimers.ContainsKey(collectionType))
        {
            _updaterTimers[collectionType].Dispose();
        }

        var updateConfig = _options.Collections.First(a => a.CollectionType == collectionType);

        var remainingInterval = TimeSpan.Zero;
        if (_collectionLog[collectionType].LastUpdated != null)
        {
            var offset = _collectionLog[collectionType].LastUpdated!.Value.Add(updateConfig.Period) - DateTime.UtcNow;
            if (offset.TotalMilliseconds > 0)
            {
                remainingInterval = offset;
            }
        }

        _updaterTimers[collectionType] = new Timer(HandleTimerAsync, updateConfig.CollectionType, remainingInterval, updateConfig.Period);
    }

    private async void HandleTimerAsync(object? stateInfo)
    {
        await RunStoreUpdateAsync((stateInfo as Type)!);
    }

    private void UpdateServiceLog(StaticStoreUpdateConfiguration updateConfig, DateTime? updateTime)
    {
        _collectionLog[updateConfig.CollectionType!] = new LastStoreUpdate
        {
            StoreName = updateConfig.StoreName,
            LastUpdated = updateTime,
            UpdateInterval = updateConfig.Period
        };
    }
}
