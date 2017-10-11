using Microsoft.Extensions.DependencyInjection;
using Voidwell.DaybreakGames.Data.DBContext;

namespace Voidwell.DaybreakGames.Data
{
    public static class DatabaseExtensions
    {
        public static IServiceCollection AddEntityFrameworkContext(this IServiceCollection services)
        {
            services.AddEntityFrameworkNpgsql();
            services.AddDbContext<PS2CharacterDbContext>();

            return services;
        }
    }
}
