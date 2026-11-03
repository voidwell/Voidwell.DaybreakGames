using Microsoft.AspNetCore.Http;
using Voidwell.DaybreakGames.Api.Middleware;
using Xunit;

namespace Voidwell.DaybreakGames.Api.Test;

public class ClientAbortMiddlewareTest
{
    [Fact]
    public async Task WhenTheClientHasAborted_ACancellationIsReportedAs499()
    {
        using var abort = new CancellationTokenSource();
        var context = new DefaultHttpContext { RequestAborted = abort.Token };
        var sut = new ClientAbortMiddleware(_ =>
        {
            abort.Cancel();
            throw new OperationCanceledException(abort.Token);
        });

        await sut.InvokeAsync(context);

        Assert.Equal(ClientAbortMiddleware.ClientClosedRequestStatusCode, context.Response.StatusCode);
    }

    [Fact]
    public async Task WhenTheClientHasNotAborted_ACancellationIsNotSwallowed()
    {
        // A cancellation with no client abort behind it (for example an internal timeout) is a real failure.
        var context = new DefaultHttpContext();
        var sut = new ClientAbortMiddleware(_ => throw new OperationCanceledException());

        await Assert.ThrowsAsync<OperationCanceledException>(() => sut.InvokeAsync(context));
    }

    [Fact]
    public async Task OtherExceptions_AreNotSwallowed_EvenIfTheClientAborted()
    {
        using var abort = new CancellationTokenSource();
        abort.Cancel();
        var context = new DefaultHttpContext { RequestAborted = abort.Token };
        var sut = new ClientAbortMiddleware(_ => throw new InvalidOperationException("boom"));

        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.InvokeAsync(context));
    }

    [Fact]
    public async Task RequestsThatComplete_AreUntouched()
    {
        var context = new DefaultHttpContext();
        var sut = new ClientAbortMiddleware(ctx =>
        {
            ctx.Response.StatusCode = StatusCodes.Status200OK;
            return Task.CompletedTask;
        });

        await sut.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
    }
}
