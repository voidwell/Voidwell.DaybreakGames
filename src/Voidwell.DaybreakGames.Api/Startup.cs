using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Voidwell.Cache;
using Voidwell.DaybreakGames.Data;
using IdentityServer4.AccessTokenValidation;
using System;
using Voidwell.Logging;
using Voidwell.DaybreakGames.CensusStore;
using Voidwell.DaybreakGames.Services;
using Voidwell.DaybreakGames.Live;
using Voidwell.DaybreakGames.Utils.HostedService;

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
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                });

            services.AddEntityFrameworkContext(Configuration);

            var applicationName = Configuration.GetValue("ApplicationName", "Voidwell.DaybreakGames");
            services.AddCache(Configuration, applicationName);

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "http://voidwellauth:5000";
                    options.SupportedTokens = SupportedTokens.Jwt;
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

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.InitializeDatabases();

            app.UseLoggingMiddleware();

            app.UseRouting();

            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
