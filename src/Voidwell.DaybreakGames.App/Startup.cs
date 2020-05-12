using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Voidwell.Cache;
using Voidwell.DaybreakGames.Data;
using System;
using Voidwell.Logging;
using Voidwell.DaybreakGames.Api;
using Voidwell.DaybreakGames.Census;
using Voidwell.DaybreakGames.Messaging;
using Voidwell.DaybreakGames.Core;
using Voidwell.DaybreakGames.GameState;

namespace Voidwell.DaybreakGames.App
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true);

            if (env.IsDevelopment())
            {
                builder.AddJsonFile("devsettings.json", true, true);
            }

            builder.AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEntityFrameworkContext(Configuration);

            var applicationName = Configuration.GetValue("ApplicationName", "Voidwell.DaybreakGames");
            services.AddCache(Configuration, applicationName);

            services.ConfigureCensusServices(Configuration);
            services.ConfigureMessagingServices(Configuration);
            services.ConfigureCoreServices();
            services.ConfigureGameStateServices();
            services.ConfigureApplicationServices(Configuration);
            services.ConfigureApiServices();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.InitializeDatabases();

            app.UseLoggingMiddleware();

            app.UseApiApplication();
        }
    }
}
