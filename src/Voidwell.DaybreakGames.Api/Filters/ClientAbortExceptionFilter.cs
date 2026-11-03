using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Voidwell.DaybreakGames.Api.Filters;

/// <summary>
/// Handles requests the client abandoned before a response was ready. Their <see cref="OperationCanceledException"/>
/// would otherwise surface as a 500 and an error log entry, although there is nobody left to respond to and nothing
/// wrong on the server.
/// </summary>
public class ClientAbortExceptionFilter : IExceptionFilter
{
    /// <summary>
    /// The non-standard "client closed request" status, used so aborted requests are not reported as server errors.
    /// </summary>
    public const int ClientClosedRequestStatusCode = 499;

    public void OnException(ExceptionContext context)
    {
        // A cancellation that the client did not cause (for example an internal timeout) is a real failure.
        if (context.Exception is OperationCanceledException && context.HttpContext.RequestAborted.IsCancellationRequested)
        {
            context.Result = new StatusCodeResult(ClientClosedRequestStatusCode);
            context.ExceptionHandled = true;
        }
    }
}
