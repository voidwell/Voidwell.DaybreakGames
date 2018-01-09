using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Exceptions;

namespace Voidwell.DaybreakGames.Census
{
    public class DaybreakGamesCensus : IDaybreakGamesCensus
    {
        private readonly CensusOptions _options;
        private readonly ILogger _logger;

        public DaybreakGamesCensus(CensusOptions options, ILogger<DaybreakGamesCensus> logger)
        {
            _options = options;
            _logger = logger;
        }

        /*

        public CensusQuery CreateQuery(string serviceName)
        {
            return new CensusQuery(this, serviceName);
        }

        internal Task<IEnumerable<JToken>> ExecuteQueryList(string queryType, CensusQuery query)
        {
            return ExecuteQueryList<JToken>(queryType, query);
        }

        internal Task<IEnumerable<T>> ExecuteQueryList<T>(string queryType, CensusQuery query)
        {
            return ExecuteQuery<IEnumerable<T>>(queryType, query);
        }

        internal Task<JToken> ExecuteQuery(string queryType, CensusQuery query)
        {
            return ExecuteQuery<JToken>(queryType, query);
        }

        internal async Task<T> ExecuteQuery<T>(string queryType, CensusQuery query)
        {
            var requestUri = CreateRequestUri(queryType, query);
            var client = new HttpClient(new CensusRetryHandler(new HttpClientHandler()));

            try
            {
                var result = await client.GetAsync(requestUri);
                var jResult = await result.GetContentAsync<JToken>();

                var jBody = jResult.SelectToken($"{query.ServiceName}_list");
                return jBody.ToObject<T>(_censusDeserializer);
            }
            catch (HttpRequestException ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                throw new CensusConnectionException($"Census query failed for query: {requestUri}: {message}");
            }
        }

        internal Task<IEnumerable<JToken>> ExecuteQueryBatch(CensusQuery query)
        {
            return ExecuteQueryBatch<JToken>(query);
        }

        internal async Task<IEnumerable<T>> ExecuteQueryBatch<T>(CensusQuery query)
        {
            var count = 0;
            List<JToken> batchResult = new List<JToken>();

            query.SetLimit(500);
            query.SetStart(count);
            query.SetLanguage("en");

            var result = await ExecuteQueryList("get", query);

            if (result.Count() < 500)
            {
                return result.Select(r => Convert<T>(r));
            }

            do
            {
                batchResult.AddRange(result);

                if (result.Count() < 500)
                {
                    return batchResult.Select(r => Convert<T>(r));
                }

                count += result.Count();
                query.SetStart(count);

                result = await ExecuteQuery("get", query);
            } while (result.Any());

            return batchResult.Select(r => Convert<T>(r));
        }

        internal Uri CreateRequestUri(string queryType, CensusQuery query)
        {
            var encArgs = query.ToString();
            return new Uri($"http://{Constants.CensusEndpoint}/s:{_options.CensusServiceKey}/{queryType}/{_options.CensusServiceNamespace}/{encArgs}");
        }

        private object GetDefault(Type type)
        {
            if (type.GetTypeInfo().IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        private T Convert<T>(JToken content)
        {
            return content.ToObject<T>(_censusDeserializer);
        }

        */
    }
}
