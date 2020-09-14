using Microsoft.Extensions.DependencyInjection;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.CensusStore.Services;

namespace Voidwell.DaybreakGames.CensusStore
{
    public static class CensusStoreExtensions
    {
        public static IServiceCollection AddCensusStores(this IServiceCollection services)
        {
            services.AddCensusHelpers();

            services.AddSingleton<CharacterStore>();
            services.AddSingleton<OutfitStore>();
            services.AddSingleton<ItemStore>();
            services.AddSingleton<VehicleStore>();
            services.AddSingleton<FactionStore>();
            services.AddSingleton<TitleStore>();
            services.AddSingleton<ProfileStore>();
            services.AddSingleton<WorldStore>();
            services.AddSingleton<ZoneStore>();

            return services;
        }
    }
}
