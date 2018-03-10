using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Voidwell.DaybreakGames.Census
{
    public class UnderscorePropertyNamesContractResolver : DefaultContractResolver
    {
        protected readonly Dictionary<string, HashSet<string>> Ignores;

        public UnderscorePropertyNamesContractResolver() : base()
        {
            Ignores = new Dictionary<string, HashSet<string>>();
        }

        public UnderscorePropertyNamesContractResolver Ignore(string type, params string[] propertyName)
        {
            if (!Ignores.ContainsKey(type)) Ignores[type] = new HashSet<string>();

            foreach (var prop in propertyName)
            {
                Ignores[type].Add(prop);
            }

            return this;
        }

        public bool IsIgnored(string type)
        {
            return Ignores.ContainsKey(type);
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (!IsIgnored(property.DeclaringType.Name))
            {
                property.PropertyName = Regex.Replace(property.PropertyName, @"(\w)([A-Z])", "$1_$2").ToLower();
            }

            return property;
        }
    }
}
