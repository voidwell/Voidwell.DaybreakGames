using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Voidwell.DaybreakGames.Data;
using IdentityServer4.AccessTokenValidation;
using System;
using Voidwell.DaybreakGames.CensusStore;
using Voidwell.DaybreakGames.Services;
using Voidwell.DaybreakGames.Live;
using Voidwell.DaybreakGames.Utils.HostedService;
using Voidwell.Microservice.Hosting;
using Voidwell.Microservice.Configuration;
using Voidwell.Microservice.Cache;
using Voidwell.Microservice.Authentication;
using Voidwell.Microservice.Tracing;

namespace Voidwell.DaybreakGames.Api
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true);

            if (env.EnvironmentName == "Development")
            {
                builder.AddJsonFile("devsettings.json", true, true);
            }

            builder.AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddMicroserviceJsonOptions();

            services.AddEntityFrameworkContext(Configuration);

            services.ConfigureServiceProperties("Voidwell.DaybreakGames");

            services.AddCache(options =>
            {
                options.RedisConfiguration = Configuration.GetValue<string>("RedisConfiguration");
            });

            //services.AddTracing();

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddServiceAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = "http://voidwellauth:5000";
                    options.SupportedTokens = Microservice.Authentication.SupportedTokens.Jwt;
                    options.RequireHttpsMetadata = false;
                    options.EnableCaching = true;
                    options.CacheDuration = TimeSpan.FromMinutes(10);
                });

            services.AddCensusServices(options =>
            {
                options.CensusServiceId = Configuration.GetValue<string>("CensusServiceKey");
                options.CensusServiceNamespace = Configuration.GetValue<string>("CensusServiceNamespace");
                options.LogCensusErrors = Configuration.GetValue<bool>("LogCensusErrors", false);
            });

            services.AddStatefulServiceDependencies();
            services.AddCensusStores(Configuration);
            services.AddApplicationServices();
            services.AddLiveServices(Configuration);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.InitializeDatabases();

            app.UseRouting();

            //app.UseTracing();
            //app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
