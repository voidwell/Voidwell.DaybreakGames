namespace Voidwell.DaybreakGames.Api.Middleware;

/// <summary>
/// Handles requests the client abandoned before a response was ready. Their <see cref="OperationCanceledException"/>
/// would otherwise surface as a 500 and an error log entry, although there is nobody left to respond to and nothing
/// wrong on the server.
/// </summary>
public class ClientAbortMiddleware
{
    /// <summary>
    /// The non-standard "client closed request" status, used so aborted requests are not reported as server errors.
    /// </summary>
    public const int ClientClosedRequestStatusCode = 499;

    private readonly RequestDelegate _next;

    public ClientAbortMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = ClientClosedRequestStatusCode;
            }
        }
    }
}

/// <summary>
/// Extension methods for <see cref="ClientAbortMiddleware"/>.
/// </summary>
public static class ClientAbortMiddlewareExtensions
{
    /// <summary>
    /// Reports requests aborted by the client as 499 instead of letting them fail with a 500. Add this after request
    /// logging, so the logged status is the 499.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The same <paramref name="app"/>.</returns>
    public static IApplicationBuilder UseClientAbortHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ClientAbortMiddleware>();
    }
}
