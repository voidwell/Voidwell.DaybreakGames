using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Reflection;

namespace Voidwell.DaybreakGames.Census
{
    public static class CensusQuery
    {
        public static string GlobalNamespace { get; set; }
        public static string GlobalApiKey { get; set; }

        public class Query : CensusObject
        {
            public string Namespace { get; set; }
            public string ApiKey { get; set; }
            public string ServiceName { get; private set; }
            private List<CensusArgument> Terms { get; set; }

            [UriQueryProperty]
            public bool ExactMatchFirst { get; set; } = false;

            [UriQueryProperty]
            private bool Timing { get; set; } = false;

            [UriQueryProperty]
            private bool IncludeNull { get; set; } = false;

            [DefaultValue(true)]
            [UriQueryProperty]
            private bool Case { get; set; } = true;

            [DefaultValue(true)]
            [UriQueryProperty]
            private bool Retry { get; set; } = true;

            [UriQueryProperty]
            private int? Limit { get; set; } = null;

            [UriQueryProperty("limitPerDB")]
            private int? LimitPerDB { get; set; } = null;

            [UriQueryProperty]
            private int Start { get; set; }

            [UriQueryProperty]
            private List<string> Show { get; set; }

            [UriQueryProperty]
            private List<string> Hide { get; set; }

            [UriQueryProperty]
            private List<string> Sort { get; set; }

            [UriQueryProperty]
            private List<string> Has { get; set; }

            [UriQueryProperty]
            private List<string> Resolve { get; set; }

            [UriQueryProperty]
            private List<CensusJoin> Join { get; set; }

            [UriQueryProperty]
            private List<CensusTree> Tree { get; set; }

            [UriQueryProperty]
            private string Distinct { get; set; }

            [UriQueryProperty("lang")]
            private string Language { get; set; }

            public Query(string serviceName, string ns = null, string apiKey = null)
            {
                ServiceName = serviceName;
                Namespace = ns ?? GlobalNamespace;
                ApiKey = apiKey ?? GlobalApiKey;
            }

            public CensusJoin JoinService(string service)
            {
                var newJoin = new CensusJoin(service);

                if (Join == null)
                {
                    Join = new List<CensusJoin>();
                }

                Join.Add(newJoin);
                return newJoin;
            }

            public CensusTree TreeField(string field)
            {
                var newTree = new CensusTree(field);

                if (Tree == null)
                {
                    Tree = new List<CensusTree>();
                }

                Tree.Add(newTree);
                return newTree;
            }

            public CensusOperand Where(string field)
            {
                var newArg = new CensusArgument(field);

                if (Terms == null)
                {
                    Terms = new List<CensusArgument>();
                }

                Terms.Add(newArg);
                return newArg.Operand;
            }

            public void ShowFields(IEnumerable<string> fields)
            {
                if (Show == null)
                {
                    Show = new List<string>();
                }

                Show.AddRange(fields);
            }

            public void HideFields(IEnumerable<string> fields)
            {
                if (Hide == null)
                {
                    Hide = new List<string>();
                }

                Hide.AddRange(fields);
            }

            public void SetLimit(int limit)
            {
                Limit = limit;
            }

            public void SetStart(int start)
            {
                Start = start;
            }

            public void AddResolve(string resolve)
            {
                if (Resolve == null)
                {
                    Resolve = new List<string>();
                }

                Resolve.Add(resolve);
            }

            public void AddResolve(IEnumerable<string> resolves)
            {
                if (Resolve == null)
                {
                    Resolve = new List<string>();
                }

                Resolve.AddRange(resolves);
            }

            public void SetLanguage(string language)
            {
                Language = language;
            }

            public async Task<T> Get<T>()
            {
                var result = await ExecuteQuery("get", this);
                return result.First.ToObject<T>(new JsonSerializer
                {
                    ContractResolver = new UnderscorePropertyNamesContractResolver()
                });
            }

            public async Task<IEnumerable<T>> GetList<T>()
            {
                var result = await ExecuteQuery("get", this);
                return result.ToObject<IEnumerable<T>>(new JsonSerializer
                {
                    ContractResolver = new UnderscorePropertyNamesContractResolver()
                });
            }

            public async Task<IEnumerable<T>> GetBatch<T>()
            {
                var result = await ExecuteQueryBatch("get", this);
                return result.ToObject<IEnumerable<T>>(new JsonSerializer
                {
                    ContractResolver = new UnderscorePropertyNamesContractResolver()
                });
            }

            public Task<JToken> Get()
            {
                return Get<JToken>();
            }

            public Task<IEnumerable<JToken>> GetList()
            {
                return GetList<JToken>();
            }

            public Task<IEnumerable<JToken>> GetBatch()
            {
                return GetBatch<JToken>();
            }

            public Uri GetUri()
            {
                return CreateRequestUri("get", this);
            }

            public override string ToString()
            {
                var baseString = base.ToString();

                var terms = Terms?.Select(t => t.ToString()).ToList() ?? new List<string>();
                var stringTerms = string.Join(GetPropertySpacer(), terms);


                if (!string.IsNullOrEmpty(baseString))
                {
                    baseString = $"?{baseString}";

                    if (!string.IsNullOrEmpty(stringTerms))
                    {
                        stringTerms = $"&{stringTerms}";
                    }
                }
                else if (!string.IsNullOrEmpty(stringTerms))
                {
                    stringTerms = $"?{stringTerms}";
                }

                return $"{ServiceName}/{baseString}{stringTerms}";
            }

            public override string GetKeyValueStringFormat()
            {
                return "c:{0}={1}";
            }

            public override string GetPropertySpacer()
            {
                return "&";
            }

            public override string GetTermSpacer()
            {
                return ",";
            }
        }

        private static Task<IEnumerable<JToken>> ExecuteQueryList(string queryType, Query query)
        {
            return ExecuteQueryList<JToken>(queryType, query);
        }

        private static async Task<IEnumerable<T>> ExecuteQueryList<T>(string queryType, Query query)
        {
            var result = await ExecuteQuery(queryType, query);
            return result.ToObject<IEnumerable<T>>();
        }

        private static async Task<JToken> ExecuteQuery(string queryType, Query query)
        {
            var requestUri = CreateRequestUri(queryType, query);
            var client = new HttpClient();

            var result = await client.GetAsync(requestUri);
            var jResult = await result.GetContentAsync<JToken>();

            return jResult.SelectToken($"{query.ServiceName}_list");
        }

        private static async Task<JToken> ExecuteQueryBatch(string queryType, Query query)
        {
            var count = 0;
            JToken batchResult = null;

            query.SetLimit(500);
            query.SetStart(count);
            query.SetLanguage("en");

            var result = await ExecuteQuery(queryType, query);

            if (result.Count() < 500)
            {
                return result;
            }

            do
            {
                if (batchResult == null)
                {
                    batchResult = result;
                }
                else
                {
                    batchResult.AddAfterSelf(result);
                }

                if (result.Count() < 500)
                {
                    return batchResult;
                }

                count += result.Count();
                query.SetStart(count);

                result = await ExecuteQuery(queryType, query);
            } while (result.HasValues);

            return batchResult;
        }

        private static Uri CreateRequestUri(string queryType, Query query)
        {
            var encArgs = query.ToString();
            return new Uri($"http://{Constants.CensusEndpoint}/s:{query.ApiKey}/{queryType}/{query.Namespace}/{encArgs}");
        }

        private static object GetDefault(Type type)
        {
            if (type.GetTypeInfo().IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
    }
}
