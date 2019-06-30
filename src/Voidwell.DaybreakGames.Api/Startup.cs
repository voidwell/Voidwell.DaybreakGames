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
using Voidwell.DaybreakGames.App;

namespace Voidwell.DaybreakGames.Api
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
            services.AddMvcCore()
                .AddDataAnnotations()
                .AddJsonFormatters(options =>
                {
                    options.NullValueHandling = NullValueHandling.Ignore;
                    options.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                });

            services.AddEntityFrameworkContext(Configuration);
            services.AddCache(Configuration, "Voidwell.DaybreakGames");

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "http://voidwellauth:5000";
                    options.SupportedTokens = SupportedTokens.Jwt;
                    options.RequireHttpsMetadata = false;

                    options.EnableCaching = true;
                    options.CacheDuration = TimeSpan.FromMinutes(10);
                });

            services.ConfigureApplicationServices(Configuration);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.InitializeDatabases();

            app.UseLoggingMiddleware();

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
