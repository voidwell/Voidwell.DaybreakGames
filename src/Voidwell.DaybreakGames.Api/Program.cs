using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Voidwell.DaybreakGames.Api.Authentication;
using Voidwell.DaybreakGames.Api.Json;
using Voidwell.DaybreakGames.Api.Logging;
using Voidwell.DaybreakGames.Api.Options;
using Voidwell.DaybreakGames.Cache;
using Voidwell.DaybreakGames.CensusStore;
using Voidwell.DaybreakGames.Data;
using Voidwell.DaybreakGames.Live;
using Voidwell.DaybreakGames.Services;
using Voidwell.DaybreakGames.Utils.HostedService;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    ApplicationName = Environment.GetEnvironmentVariable("ApplicationName") ?? "Voidwell.DaybreakGames"
});

builder.WebHost.UseUrls("http://0.0.0.0:5000");

builder.Logging.AddApiLogging(builder.Environment, builder.Configuration);

var configuration = builder.Configuration;
var services = builder.Services;

services.AddControllers()
    .AddApiJsonOptions();

services.AddEntityFrameworkContext(configuration);

services.AddCache(options =>
{
    options.RedisConfiguration = configuration.GetValue<string>("RedisConfiguration");
    options.KeyPrefix = "Voidwell.DaybreakGames";
});

var authOptions = configuration.GetSection("Auth").Get<AuthOptions>() ?? throw new InvalidOperationException("Auth configuration section is missing.");

services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddServiceAuthentication(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.Authority = authOptions.Authority;
        options.ClientId = authOptions.ClientId;
        options.ClientSecret = authOptions.ClientSecret;
        options.RoleClaimType = authOptions.RoleClaimType;
        options.SupportedTokens = SupportedTokens.Both;
        options.RequireHttpsMetadata = false;
        options.EnableCaching = true;
        options.CacheDuration = TimeSpan.FromMinutes(2);
    });

services.AddAuthorizationBuilder()
    .AddPolicy(AuthConstants.Policies.Mutterblack, policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim(JwtClaimTypes.ClientId, "mutterblack");
    });

var allowedOrigins = configuration.GetValue<string>("OriginAddress");
services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(new[] { "http://localhost:4200", allowedOrigins }.Where(o => !string.IsNullOrEmpty(o)).ToArray()!)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

services.AddCensusServices(options =>
{
    options.CensusServiceId = configuration.GetValue<string>("CensusServiceKey")!;
    options.CensusServiceNamespace = configuration.GetValue<string>("CensusServiceNamespace")!;
    options.LogCensusErrors = configuration.GetValue<bool>("LogCensusErrors", false);
});

services.AddStatefulServiceDependencies();
services.AddCensusStores(configuration);
services.AddApplicationServices();
services.AddLiveServices(configuration);

var app = builder.Build();

app.InitializeDatabases();

app.UseForwardedHeaders(GetForwardedHeaderOptions());

app.UseRouting();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

static ForwardedHeadersOptions GetForwardedHeaderOptions()
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
