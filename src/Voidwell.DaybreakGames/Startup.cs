using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Voidwell.DaybreakGames.Census;
using Newtonsoft.Json;
using Voidwell.DaybreakGames.Data;
using Voidwell.DaybreakGames.Services.Planetside;
using Voidwell.DaybreakGames.Websocket;
using Newtonsoft.Json.Serialization;
using Voidwell.Cache;
using Voidwell.DaybreakGames.CensusServices;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using Voidwell.DaybreakGames.Services;

namespace Voidwell.DaybreakGames
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
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
                });

            services.AddCache("DaybreakGames");
            services.AddEntityFrameworkContext(Configuration);
            services.AddCensusClient(Configuration);
            services.AddCensusServices();
            services.AddUpdateableTasks();

            services.AddOptions();
            services.AddSingleton(impl => impl.GetRequiredService<IOptions<DaybreakGamesOptions>>().Value);
            services.Configure<DaybreakGamesOptions>(Configuration);

            services.AddTransient<ICharacterService, CharacterService>();
            services.AddTransient<IOutfitService, OutfitService>();
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
            services.AddTransient<IWorldMonitor, WorldMonitor>();
            services.AddTransient<IUpdaterService, UpdaterService>();
            services.AddTransient<IFactionService, FactionService>();
            services.AddTransient<IFeedService, FeedService>();

            services.AddSingleton<IWebsocketEventHandler, WebsocketEventHandler>();
            services.AddSingleton<IWebsocketMonitor, WebsocketMonitor>();
            services.AddSingleton<IHostedService, StoreUpdaterScheduler>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory
                .WithFilter(new FilterLoggerSettings
                {
                    { "Microsoft", LogLevel.Error }
                })
                .AddConsole(Configuration.GetSection("Logging"))
                .AddDebug();

            app.UseMvc();
        }
    }
}
