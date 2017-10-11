using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Voidwell.DaybreakGames.Census;
using Newtonsoft.Json;
using Voidwell.DaybreakGames.Data;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore()
                .AddDataAnnotations()
                .AddJsonFormatters(options =>
                {
                    options.NullValueHandling = NullValueHandling.Ignore;
                });

            services.AddEntityFrameworkContext();

            services.AddOptions();
            services.Configure<DaybreakAPIOptions>(Configuration);
            services.AddTransient(a => a.GetRequiredService<IOptions<DaybreakAPIOptions>>().Value);

            services.AddSingleton<ICharacterService, CharacterService>();
            services.AddSingleton<IOutfitService, OutfitService>();

            CensusQuery.GlobalApiKey = "example";
            CensusQuery.GlobalNamespace = "ps2";
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();
        }
    }
}
