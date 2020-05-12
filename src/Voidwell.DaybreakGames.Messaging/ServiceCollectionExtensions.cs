using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Voidwell.DaybreakGames.Utils.HttpAuthenticatedClient;

namespace Voidwell.DaybreakGames.Messaging
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureMessagingServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthenticatedHttpClient(options =>
            {
                options.TokenServiceAddress = "http://voidwellauth:5000/connect/token";
                options.ClientId = configuration.GetValue<string>("ClientId");
                options.ClientSecret = configuration.GetValue<string>("ClientSecret");
                options.Scopes = new List<string>
                    {
                        "voidwell-messagewell-publish"
                    };
            });

            services.AddOptions();
            services.Configure<MessagingOptions>(configuration);

            services.AddSingleton<IMessageService, MessageService>();
        }
    }
}
