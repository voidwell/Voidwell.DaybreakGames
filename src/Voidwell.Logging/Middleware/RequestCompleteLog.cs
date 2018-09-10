using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Voidwell.Logging.Middleware
{
    internal class RequestCompleteLog : IReadOnlyList<KeyValuePair<string, object>>
    {
        internal static readonly Func<object, Exception, string> Callback = 
            (state, exception) => ((RequestCompleteLog)state).ToString();

        private readonly TimeSpan _elapsed;
        private readonly HttpContext _httpContext;

        private string _cachedToString;

        public RequestCompleteLog(HttpContext httpContext, TimeSpan elapsed)
        {
            _httpContext = httpContext;
            _elapsed = elapsed;
        }

        public int Count => 4;

        public KeyValuePair<string, object> this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return new KeyValuePair<string, object>("ElapsedMilliseconds", _elapsed.TotalMilliseconds);
                    case 1:
                        return new KeyValuePair<string, object>("StatusCode", _httpContext.Response.StatusCode);
                    case 2:
                        return new KeyValuePair<string, object>("ContentType", _httpContext.Response.ContentType);
                    case 3:
                        return new KeyValuePair<string, object>("Method", _httpContext.Request.Method);
                    default:
                        throw new IndexOutOfRangeException(nameof(index));
                }
            }
        }

        public override string ToString()
        {
            if (_cachedToString == null)
            {
                _cachedToString = string.Format(
                    CultureInfo.InvariantCulture,
                    "Request {0} {1} finished in {2}ms ({3})",
                    _httpContext.Request.Method.ToUpper(),
                    _httpContext.Request.Path,
                    _elapsed.TotalMilliseconds,
                    _httpContext.Response.StatusCode);
            }

            return _cachedToString;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
