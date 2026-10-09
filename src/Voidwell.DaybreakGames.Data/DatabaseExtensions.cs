using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.Data;

public static class DatabaseExtensions
{
    public static IServiceCollection AddEntityFrameworkContext(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.Get<DatabaseOptions>() ?? new DatabaseOptions();

        services.AddPooledDbContextFactory<PS2DbContext>(
            builder => PS2DbContext.Configure(builder, options),
            options.PoolSize);

        services.AddSingleton<IUpdaterSchedulerRepository, UpdaterSchedulerRepository>();
        services.AddSingleton<IFactionRepository, FactionRepository>();
        services.AddSingleton<IItemRepository, ItemRepository>();
        services.AddSingleton<IZoneRepository, ZoneRepository>();
        services.AddSingleton<IWorldRepository, WorldRepository>();
        services.AddSingleton<IVehicleRepository, VehicleRepository>();
        services.AddSingleton<ITitleRepository, TitleRepository>();
        services.AddSingleton<IProfileRepository, ProfileRepository>();
        services.AddSingleton<ILoadoutRepository, LoadoutRepository>();
        services.AddSingleton<IMetagameEventRepository, MetagameEventRepository>();
        services.AddSingleton<IMapRepository, MapRepository>();
        services.AddSingleton<ICharacterRepository, CharacterRepository>();
        services.AddSingleton<IOutfitRepository, OutfitRepository>();
        services.AddSingleton<IPlayerSessionRepository, PlayerSessionRepository>();
        services.AddSingleton<IEventRepository, EventRepository>();
        services.AddSingleton<IAlertRepository, AlertRepository>();
        services.AddSingleton<ICharacterUpdaterRepository, CharacterUpdaterRepository>();
        services.AddSingleton<IFunctionalRepository, FunctionalRepository>();
        services.AddSingleton<ISanctionedWeaponsRepository, SanctionedWeaponsRepository>();
        services.AddSingleton<IExperienceRepository, ExperienceRepository>();
        services.AddSingleton<IWeaponAggregateRepository, WeaponAggregateRepository>();
        services.AddSingleton<IDirectiveRepository, DirectiveRepository>();
        services.AddSingleton<IObjectiveRepository, ObjectiveRepository>();
        services.AddSingleton<IRewardRepository, RewardRepository>();
        services.AddSingleton<ICharacterDirectiveRepository, CharacterDirectiveRepository>();

        return services;
    }

    public static IApplicationBuilder InitializeDatabases(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();

        var dbContextFactory = serviceScope.ServiceProvider.GetRequiredService<IDbContextFactory<PS2DbContext>>();
        using var dbContext = dbContextFactory.CreateDbContext();
        dbContext.Database.Migrate();

        return app;
    }
}
