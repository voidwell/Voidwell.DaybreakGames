using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Voidwell.DaybreakGames.Api.Filters;
using Xunit;

namespace Voidwell.DaybreakGames.Api.Test;

public class ClientAbortExceptionFilterTest
{
    private static ExceptionContext CreateContext(Exception exception, bool clientAborted)
    {
        using var abort = new CancellationTokenSource();
        if (clientAborted)
        {
            abort.Cancel();
        }

        var httpContext = new DefaultHttpContext { RequestAborted = abort.Token };
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

        return new ExceptionContext(actionContext, []) { Exception = exception };
    }

    [Fact]
    public void WhenTheClientHasAborted_ACancellationIsReportedAs499()
    {
        var context = CreateContext(new OperationCanceledException(), clientAborted: true);

        new ClientAbortExceptionFilter().OnException(context);

        var result = Assert.IsType<StatusCodeResult>(context.Result);
        Assert.Equal(ClientAbortExceptionFilter.ClientClosedRequestStatusCode, result.StatusCode);
        Assert.True(context.ExceptionHandled);
    }

    [Fact]
    public void ATaskCanceledException_FromAnAbortedRequest_IsAlsoHandled()
    {
        var context = CreateContext(new TaskCanceledException(), clientAborted: true);

        new ClientAbortExceptionFilter().OnException(context);

        Assert.True(context.ExceptionHandled);
    }

    [Fact]
    public void WhenTheClientHasNotAborted_ACancellationIsNotHandled()
    {
        // A cancellation with no client abort behind it (for example an internal timeout) is a real failure.
        var context = CreateContext(new OperationCanceledException(), clientAborted: false);

        new ClientAbortExceptionFilter().OnException(context);

        Assert.Null(context.Result);
        Assert.False(context.ExceptionHandled);
    }

    [Fact]
    public void OtherExceptions_AreNotHandled_EvenIfTheClientAborted()
    {
        var context = CreateContext(new InvalidOperationException("boom"), clientAborted: true);

        new ClientAbortExceptionFilter().OnException(context);

        Assert.Null(context.Result);
        Assert.False(context.ExceptionHandled);
    }
}
