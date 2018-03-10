using System;
using System.Collections.Generic;
using Voidwell.DaybreakGames.Census.JsonConverters;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Reflection;
using Voidwell.DaybreakGames.Census.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Voidwell.DaybreakGames.Census
{
    public class CensusClient : ICensusClient
    {
        private readonly CensusOptions _options;
        private readonly ILogger _logger;

        private JsonSerializer _censusDeserializer { get; set; }
        private HttpClient _client { get; set; }

        private const int _batchLimit = 500;

        public CensusClient(IOptions<CensusOptions> options, ILogger<CensusClient> logger)
        {
            _options = options.Value;
            _logger = logger;

            var resolver = new UnderscorePropertyNamesContractResolver()
                .Ignore("CensusMapModel")
                .Ignore("CensusMapRegionSet")
                .Ignore("CensusMapRegionSetRow")
                .Ignore("CensusMapRegionSetRowData");

            var settings = new JsonSerializerSettings
            {
                ContractResolver = resolver,
                Converters = new JsonConverter[]
                {
                    new BooleanJsonConverter(),
                    new DateTimeJsonConverter()
                }
            };
            _censusDeserializer = JsonSerializer.Create(settings);
            _client = new HttpClient(new CensusRetryHandler(new HttpClientHandler(), _logger));
        }

        public CensusQuery CreateQuery(string serviceName)
        {
            return new CensusQuery(this, serviceName);
        }

        internal Task<IEnumerable<T>> ExecuteQueryList<T>(CensusQuery query, bool throwError)
        {
            return ExecuteQuery<IEnumerable<T>>(query, throwError);
        }

        internal async Task<T> ExecuteQuery<T>(CensusQuery query, bool throwError)
        {
            var requestUri = CreateRequestUri(query);

            try {
                var result = await _client.GetAsync(requestUri);
                var jResult = await result.GetContentAsync<JToken>();

                var errorCode = jResult.Value<string>("errorCode");
                if (errorCode != null)
                {
                    var errorMessage = jResult.Value<string>("errorMessage");

                    _logger.LogError(05417, errorMessage);

                    if (throwError)
                    {
                        throw new CensusServerException($"{errorCode}: {errorMessage}");
                    }
                }
                else
                {
                    var jBody = jResult.SelectToken($"{query.ServiceName}_list");
                    return jBody.ToObject<T>(_censusDeserializer);
                }
            }
            catch(HttpRequestException ex)
            {
                var exMessage = ex.InnerException?.Message ?? ex.Message;
                var errorMessage = $"Census query failed for query: {requestUri}: {exMessage}";

                _logger.LogError(05418, errorMessage);

                if (throwError)
                {
                    throw new CensusConnectionException(errorMessage);
                }
            }

            return default(T);
        }

        internal async Task<IEnumerable<T>> ExecuteQueryBatch<T>(CensusQuery query, bool throwError)
        {
            var count = 0;
            List<JToken> batchResult = new List<JToken>();

            query.SetLimit(_batchLimit);
            query.SetStart(count);
            query.SetLanguage("en");

            var result = await ExecuteQueryList<JToken>(query, true);

            if (result.Count() < _batchLimit)
            {
                return result.Select(r => Convert<T>(r));
            }

            do
            {
                batchResult.AddRange(result);

                if (result.Count() < _batchLimit)
                {
                    return batchResult.Select(r => Convert<T>(r));
                }

                count += result.Count();
                query.SetStart(count);

                result = await ExecuteQuery<JToken>(query, true);
            } while (result.Any());

            return batchResult.Select(r => Convert<T>(r));
        }

        internal Uri CreateRequestUri(CensusQuery query)
        {
            var encArgs = query.ToString();
            return new Uri($"http://{Constants.CensusEndpoint}/s:{_options.CensusServiceKey}/get/{_options.CensusServiceNamespace}/{encArgs}");
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
    }
}
