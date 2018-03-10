using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.Census
{
    public sealed class CensusQuery : CensusObject
    {
        private readonly CensusClient _censusClient;

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

        public CensusQuery(CensusClient censusClient, string serviceName)
        {
            _censusClient = censusClient;

            ServiceName = serviceName;
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

        public async Task<T> Get<T>(bool throwError = true)
        {
            var result = await _censusClient.ExecuteQuery<IEnumerable<T>>(this, throwError);
            return result.FirstOrDefault();
        }

        public Task<IEnumerable<T>> GetList<T>(bool throwError = true)
        {
            return _censusClient.ExecuteQuery<IEnumerable<T>>(this, throwError);
        }

        public Task<IEnumerable<T>> GetBatch<T>(bool throwError = true)
        {
            return _censusClient.ExecuteQueryBatch<T>(this, throwError);
        }

        public Uri GetUri()
        {
            return _censusClient.CreateRequestUri(this);
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
}
