using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Voidwell.DaybreakGames.HttpAuthenticatedClient
{
    public static class AuthenticatedHttpClientExtensions
    {
        public static void AddAuthenticatedHttpClient(this IServiceCollection services, Action<AuthenticatedHttpClientOptions> setupAction = null)
        {
            var options = new AuthenticatedHttpClientOptions();
            setupAction?.Invoke(options);
            services.AddSingleton(options);

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<AuthenticatedHttpMessageHandler>();

            services.AddTransient<Func<AuthenticatedHttpMessageHandler>>(
                sp => () => sp.GetService<AuthenticatedHttpMessageHandler>());

            services.AddTransient<IAuthenticatedHttpClientFactory, AuthenticatedHttpClientFactory>();
        }
    }
}
