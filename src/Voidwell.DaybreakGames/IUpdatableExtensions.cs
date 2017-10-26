using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;
using Voidwell.DaybreakGames.Services;

namespace Voidwell.DaybreakGames
{
    public static class IUpdatableExtensions
    {
        public static IServiceCollection AddUpdateableTasks(this IServiceCollection services)
        {
            var updatableTypes = typeof(IUpdateable).GetTypeInfo().Assembly.GetTypes()
                .Where(a => typeof(IUpdateable).IsAssignableFrom(a) && a.GetTypeInfo().IsClass && !a.GetTypeInfo().IsAbstract)
                .ToList();

            updatableTypes.ForEach(t => services.AddTransient(typeof(IUpdateable), t));

            return services;
        }
    }
}
