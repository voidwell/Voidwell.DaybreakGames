using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
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
                    options.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

            services.AddCache(Configuration, "Voidwell.DaybreakGames");
            services.AddEntityFrameworkContext(Configuration);

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.Authority = "http://voidwellauth:5000";
                o.Audience = "voidwell-daybreakgames";
                o.RequireHttpsMetadata = false;
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false
                };

                var validator = o.SecurityTokenValidators.OfType<JwtSecurityTokenHandler>().SingleOrDefault();
                validator.InboundClaimTypeMap = new Dictionary<string, string>();
                validator.OutboundClaimTypeMap = new Dictionary<string, string>();
            });

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
            services.AddTransient<ISearchService, SearchService>();

            services.AddSingleton<IWebsocketEventHandler, WebsocketEventHandler>();
            //services.AddSingleton<IWebsocketMonitor, WebsocketMonitor>();
            services.AddSingleton<IHostedService, StoreUpdaterScheduler>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //app.InitializeDatabases();

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
