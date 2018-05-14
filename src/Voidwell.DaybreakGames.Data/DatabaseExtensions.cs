using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Reflection;
using Voidwell.DaybreakGames.Data.Repositories;
using System;

namespace Voidwell.DaybreakGames.Data
{
    public static class DatabaseExtensions
    {
        private static string _migrationAssembly = typeof(DatabaseExtensions).GetTypeInfo().Assembly.GetName().Name;

        public static IServiceCollection AddEntityFrameworkContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.AddSingleton(impl => impl.GetRequiredService<IOptions<DatabaseOptions>>().Value);
            services.Configure<DatabaseOptions>(configuration);

            var options = configuration.Get<DatabaseOptions>();

            services.AddEntityFrameworkNpgsql();

            services.AddDbContextPool<PS2DbContext>(builder =>
                builder.UseNpgsql(options.DBConnectionString, b => {
                    b.MigrationsAssembly(_migrationAssembly);
                }), 100);

            services.AddSingleton<IDbContextHelper, DbContextHelper>();
            services.AddSingleton<IUpdaterSchedulerRepository, UpdaterSchedulerRepository>();
            services.AddSingleton<IFactionRepository, FactionRepository>();
            services.AddSingleton<IItemRepository, ItemRepository>();
            services.AddSingleton<IZoneRepository, ZoneRepository>();
            services.AddSingleton<IWorldRepository, WorldRepository>();
            services.AddSingleton<IVehicleRepository, VehicleRepository>();
            services.AddSingleton<ITitleRepository, TitleRepository>();
            services.AddSingleton<IProfileRepository, ProfileRepository>();
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

            return services;
        }

        public static IApplicationBuilder InitializeDatabases(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<PS2DbContext>();
                dbContext.Database.Migrate();
            }

            return app;
        }
    }
}
