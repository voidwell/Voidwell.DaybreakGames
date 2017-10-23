using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Voidwell.DaybreakGames.Data.DBContext;

namespace Voidwell.DaybreakGames.Data
{
    public static class DatabaseExtensions
    {
        public static IServiceCollection AddEntityFrameworkContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.AddSingleton(impl => impl.GetRequiredService<IOptions<DatabaseOptions>>().Value);
            services.Configure<DatabaseOptions>(configuration);

            services.AddEntityFrameworkNpgsql();
            services.AddDbContext<PS2DbContext>(ServiceLifetime.Scoped);

            return services;
        }
    }
}
