using Microsoft.AspNetCore.Authentication;

namespace Voidwell.DaybreakGames.Api.Authentication;

public enum SupportedTokens
{
    Both = 0,
    Jwt = 1,
    Reference = 2
}

public class ServiceAuthenticationOptions
{
    public string? Authority { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public SupportedTokens SupportedTokens { get; set; }
    public bool SaveToken { get; set; }
    public TimeSpan CacheDuration { get; set; }
    public string? RoleClaimType { get; set; }
    public string? NameClaimType { get; set; }
    public bool RequireHttpsMetadata { get; set; }
    public bool EnableCaching { get; set; }
    public string? Audience { get; set; }
    public HttpMessageHandler? BackchannelHttpHandler { get; set; }
}

public static class ServiceAuthenticationExtensions
{
    private const string _introspectionScheme = "introspection";

    public static AuthenticationBuilder AddServiceAuthentication(this AuthenticationBuilder builder, string authenticationScheme, Action<ServiceAuthenticationOptions> optionsAction)
    {
        var options = new ServiceAuthenticationOptions();
        optionsAction.Invoke(options);

        if (options.SupportedTokens != SupportedTokens.Reference)
        {
            builder.AddJwtBearer(authenticationScheme, o =>
            {
                o.Authority = options.Authority;
                o.Audience = options.Audience;
                o.SaveToken = options.SaveToken;
                o.RequireHttpsMetadata = options.RequireHttpsMetadata;
                o.BackchannelHttpHandler = options.BackchannelHttpHandler;

                if (!string.IsNullOrEmpty(options.RoleClaimType))
                {
                    o.TokenValidationParameters.RoleClaimType = options.RoleClaimType;
                }

                if (options.SupportedTokens == SupportedTokens.Both)
                {
                    o.ForwardDefaultSelector = ForwardReferenceToken(_introspectionScheme);
                }
            });
        }

        if (options.SupportedTokens != SupportedTokens.Jwt)
        {
            builder.AddOAuth2Introspection(options.SupportedTokens == SupportedTokens.Reference ? authenticationScheme : _introspectionScheme, o =>
            {
                o.Authority = options.Authority;
                o.ClientId = options.ClientId;
                o.ClientSecret = options.ClientSecret;
                o.SaveToken = options.SaveToken;
                o.EnableCaching = options.EnableCaching;
                o.CacheDuration = options.CacheDuration;
                o.NameClaimType = options.NameClaimType;
                o.RoleClaimType = options.RoleClaimType;
            });
        }

        return builder;
    }

    private static Func<HttpContext, string?> ForwardReferenceToken(string introspectionScheme)
    {
        return context =>
        {
            var (scheme, credential) = GetSchemeAndCredential(context);

            if (scheme.Equals("Bearer", StringComparison.OrdinalIgnoreCase) && !credential.Contains('.'))
            {
                return introspectionScheme;
            }

            return null;
        };
    }

    private static (string, string) GetSchemeAndCredential(HttpContext context)
    {
        var header = context.Request.Headers["Authorization"].FirstOrDefault();

        if (string.IsNullOrEmpty(header))
        {
            return ("", "");
        }

        var parts = header.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
        {
            return ("", "");
        }

        return (parts[0], parts[1]);
    }
}
