using Voidwell.DaybreakGames.Census.Platforms;
using Voidwell.DaybreakGames.Utils;

namespace Voidwell.DaybreakGames.Api.Middleware;

/// <summary>
/// Sets the request's Census platform from the <c>platform</c> query parameter or the <c>X-Platform</c> header.
/// Requests without either stay unscoped: lists cover every platform and lookups find the entity's own platform.
/// </summary>
public class CensusPlatformMiddleware
{
    public const string QueryParameterName = "platform";
    public const string HeaderName = "X-Platform";

    private readonly RequestDelegate _next;

    public CensusPlatformMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ICensusContext censusContext)
    {
        string? value = context.Request.Query[QueryParameterName];
        if (string.IsNullOrWhiteSpace(value))
        {
            value = context.Request.Headers[HeaderName];
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            await _next(context);
            return;
        }

        if (!Enum.TryParse<Platform>(value, true, out var platform) || !Enum.IsDefined(platform))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync($"Unknown platform '{value}'. Valid values: {string.Join(", ", Enum.GetNames<Platform>())}.");
            return;
        }

        using (censusContext.Use(platform))
        {
            await _next(context);
        }
    }
}
