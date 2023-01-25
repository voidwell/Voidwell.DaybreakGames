using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Voidwell.Cache;

namespace Voidwell.DaybreakGames.Utils.HostedService
{
    public class StatefulHostedServiceManager : IStatefulHostedServiceManager
    {
        private readonly ICache _cache;
        private readonly IEnumerable<HostedServiceState> _services;

        private readonly Func<string, string> _cacheKey = (serviceName) => $"service-{serviceName}-state";

        public StatefulHostedServiceManager(IServiceProvider serviceProvider, ICache cache)
        {
            _cache = cache;

            _services = serviceProvider.GetServices(typeof(HostedServiceState<>))
                .Cast<HostedServiceState>();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.WhenAll(_services.Select(a => InitializeServiceAsync(a)));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task<ServiceState> GetServiceStatusAsync(string serviceName, CancellationToken cancellationToken)
        {
            var state = GetState(serviceName);
            if (state == null)
            {
                return null;
            }

            return await GetServiceStatusAsync(state, cancellationToken);
        }

        public async Task<IEnumerable<ServiceState>> GetServiceStatusAsync(CancellationToken cancellationToken)
        {
            return await Task.WhenAll(_services.Select(a => GetServiceStatusAsync(a, cancellationToken)));
        }

        public async Task StartServiceAsync(string serviceName, CancellationToken cancellationToken)
        {
            var state = GetState(serviceName);
            if (state != null)
            {
                await Task.WhenAll(UpdateStateAsync(state, true), state.Service.StartAsync(cancellationToken));
            }
        }

        public async Task StopServiceAsync(string serviceName, CancellationToken cancellationToken)
        {
            var state = GetState(serviceName);
            if (state != null)
            {
                await Task.WhenAll(UpdateStateAsync(state, false), state.Service.StopAsync(cancellationToken));
            }
        }

        public bool VerifyServiceExists(string serviceName)
        {
            return GetState(serviceName) != null;
        }

        private async Task UpdateStateAsync(HostedServiceState serviceState, bool isEnabled)
        {
            serviceState.IsRunning = isEnabled;

            var state = await GetServiceStatusAsync(serviceState, CancellationToken.None);
            await _cache.SetAsync(_cacheKey(serviceState.ServiceName), state);
        }

        private async Task InitializeServiceAsync(HostedServiceState serviceState)
        {
            var state = await _cache.GetAsync<ServiceState>(_cacheKey(serviceState.ServiceName));
            if (state != null && state.IsEnabled)
            {
                serviceState.IsRunning = true;
                await serviceState.Service.StartAsync(CancellationToken.None);
            }
        }

        private async Task<ServiceState> GetServiceStatusAsync(HostedServiceState serviceState, CancellationToken cancellationToken)
        {
            var details = await serviceState.Service.GetStatusAsync(cancellationToken);

            return new ServiceState
            {
                Name = serviceState.ServiceName,
                IsEnabled = serviceState.IsRunning,
                Details = details
            };
        }

        private HostedServiceState GetState(string serviceName)
        {
            return _services.FirstOrDefault(a => a.ServiceName == serviceName);
        }
    }
}
