using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Voidwell.DaybreakGames.Api.Authentication;
using Voidwell.DaybreakGames.Api.Json;
using Voidwell.DaybreakGames.Cache;
using Voidwell.DaybreakGames.CensusStore;
using Voidwell.DaybreakGames.Data;
using Voidwell.DaybreakGames.Live;
using Voidwell.DaybreakGames.Services;
using Voidwell.DaybreakGames.Utils.HostedService;

namespace Voidwell.DaybreakGames.Api;

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
            .AddApiJsonOptions();

        services.AddEntityFrameworkContext(Configuration);

        services.AddCache(options =>
        {
            options.RedisConfiguration = Configuration.GetValue<string>("RedisConfiguration");
            options.KeyPrefix = "Voidwell.DaybreakGames";
        });

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddServiceAuthentication(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = "https://auth.voidwell.com";
                options.ClientId = "voidwell-daybreakgames";
                options.ClientSecret = Configuration.GetValue<string>("ApiResourceSecret");
                options.SupportedTokens = SupportedTokens.Both;
                options.RequireHttpsMetadata = false;
                options.EnableCaching = true;
                options.CacheDuration = TimeSpan.FromMinutes(2);
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthConstants.Policies.Mutterblack, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(JwtClaimTypes.ClientId, "mutterblack");
            });
        });

        services.AddMemoryCache();
        services.AddAuthenticatedHttpClient<IUserRolesClient, UserRolesClient>(options =>
        {
            options.TokenServiceAddress = "https://auth.voidwell.com/connect/token";
            options.ClientId = "voidwell-daybreakgames";
            options.ClientSecret = Configuration.GetValue<string>("ClientSecret");
            options.Scopes = new List<string> { "voidwell-usermanagement" };
        });
        services.AddSingleton<IClaimsTransformation, RoleClaimsTransformation>();

        var allowedOrigins = Configuration.GetValue<string>("OriginAddress");
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.WithOrigins(new[] { "http://localhost:4200", allowedOrigins }.Where(o => !string.IsNullOrEmpty(o)).ToArray()!)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        services.AddCensusServices(options =>
        {
            options.CensusServiceId = Configuration.GetValue<string>("CensusServiceKey")!;
            options.CensusServiceNamespace = Configuration.GetValue<string>("CensusServiceNamespace")!;
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

        app.UseForwardedHeaders(GetForwardedHeaderOptions());

        app.UseRouting();

        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }

    private static ForwardedHeadersOptions GetForwardedHeaderOptions()
    {
        var options = new ForwardedHeadersOptions
        {
            RequireHeaderSymmetry = false,
            ForwardLimit = 15,
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
        };

        options.KnownIPNetworks.Clear();
        options.KnownProxies.Clear();

        return options;
    }
}
