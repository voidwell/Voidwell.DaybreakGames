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

            services.AddOptions();
            services.AddSingleton(impl => impl.GetRequiredService<IOptions<DaybreakGamesOptions>>().Value);
            services.Configure<DaybreakGamesOptions>(Configuration);

            services.AddScoped<ICharacterService, CharacterService>();
            services.AddScoped<IOutfitService, OutfitService>();
            services.AddScoped<IItemService, ItemService>();
            services.AddScoped<IMapService, MapService>();
            services.AddScoped<IFactionService, FactionService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<ITitleService, TitleService>();
            services.AddScoped<IVehicleService, VehicleService>();
            services.AddScoped<IWorldService, WorldService>();
            services.AddScoped<IZoneService, ZoneService>();
            services.AddScoped<IWeaponService, WeaponService>();
            services.AddScoped<IAlertService, AlertService>();
            services.AddScoped<ICombatReportService, CombatReportService>();
            services.AddScoped<IMetagameEventService, MetagameEventService>();
            services.AddScoped<IWorldMonitor, WorldMonitor>();
            services.AddScoped<IUpdaterService, UpdaterService>();
            services.AddScoped<IWebsocketEventHandler, WebsocketEventHandler>();
            services.AddScoped<IWebsocketMonitor, WebsocketMonitor>();
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
