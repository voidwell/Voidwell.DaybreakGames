using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Voidwell.Cache;
using Voidwell.DaybreakGames.Data;
using Voidwell.DaybreakGames.Services.Planetside;
using Voidwell.DaybreakGames.Websocket;
using Voidwell.DaybreakGames.Census;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.Services;
using Microsoft.Extensions.Hosting;
using Voidwell.DaybreakGames.HostedServices;
using IdentityServer4.AccessTokenValidation;

namespace Voidwell.DaybreakGames
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

            services.AddOptions();
            services.Configure<DaybreakGamesOptions>(Configuration);
            services.Configure<DaybreakGamesOptions>(a =>
            {
                var eventNames = Configuration.GetValue<string>("CensusWebsocketServices");
                var experienceIds = Configuration.GetValue<string>("CensusWebsocketExperienceIds");

                a.CensusWebsocketServices = eventNames?.Replace(" ", "").Split(",");
                a.CensusWebsocketExperienceIds = experienceIds?.Replace(" ", "").Split(",");
            });

            services.AddEntityFrameworkContext(Configuration);
            services.AddCache(Configuration, "Voidwell.DaybreakGames");
            services.AddCensusClient(Configuration);
            services.AddCensusServices();
            services.AddUpdateableTasks();

            services.AddTransient<IItemService, ItemService>();
            services.AddTransient<IMapService, MapService>();
            services.AddTransient<IProfileService, ProfileService>();
            services.AddTransient<ITitleService, TitleService>();
            services.AddTransient<IVehicleService, VehicleService>();
            services.AddTransient<IWorldService, WorldService>();
            services.AddTransient<IZoneService, ZoneService>();
            services.AddTransient<IWeaponService, WeaponService>();
            services.AddTransient<IAlertService, AlertService>();
            services.AddTransient<ICombatReportService, CombatReportService>();
            services.AddTransient<IMetagameEventService, MetagameEventService>();
            services.AddTransient<IFactionService, FactionService>();
            services.AddTransient<IFeedService, FeedService>();
            services.AddTransient<ISearchService, SearchService>();
            services.AddTransient<IGradeService, GradeService>();

            services.AddSingleton<ICharacterService, CharacterService>();
            services.AddSingleton<IOutfitService, OutfitService>();
            services.AddSingleton<IWorldMonitor, WorldMonitor>();
            services.AddSingleton<IWeaponAggregateService, WeaponAggregateService>();
            services.AddSingleton<IPSBUtilityService, PSBUtilityService>();

            services.AddSingleton<ICharacterUpdaterService, CharacterUpdaterService>();
            services.AddSingleton<IWebsocketEventHandler, WebsocketEventHandler>();
            services.AddSingleton<IWebsocketMonitor, WebsocketMonitor>();

            services.AddSingleton<IHostedService, StoreUpdaterSchedulerHostedService>();
            services.AddSingleton<IHostedService, WebsocketMonitorHostedService>();
            services.AddSingleton<IHostedService, CharacterUpdaterHostedService>();

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "http://voidwellauth:5000";
                    options.SupportedTokens = SupportedTokens.Jwt;
                    options.RequireHttpsMetadata = false;
                });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.InitializeDatabases();

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
