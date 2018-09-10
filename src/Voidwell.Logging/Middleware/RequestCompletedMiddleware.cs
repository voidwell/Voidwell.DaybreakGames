using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Voidwell.Logging.Middleware
{
    public class RequestCompletedMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestCompletedMiddleware> _logger;

        public RequestCompletedMiddleware(RequestDelegate next, ILogger<RequestCompletedMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var sw = new Stopwatch();
            sw.Start();

            await _next(httpContext);

            sw.Stop();

            _logger.Log(LogLevel.Information, 10, new RequestCompleteLog(httpContext, sw.Elapsed),
                null, RequestCompleteLog.Callback);
        }
    }
}
