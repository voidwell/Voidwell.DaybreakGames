using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Voidwell.Logging
{
    public static class GelfMessageExtensions
    {
        public static string ToJson(this GelfMessage message)
        {
            var messageJson = JObject.FromObject(message);

            foreach (var field in message.AdditionalFields)
            {
                if (IsNumeric(field.Value))
                {
                    messageJson[$"_{field.Key}"] = JToken.FromObject(field.Value);
                }
                else
                {
                    messageJson[$"_{field.Key}"] = field.Value?.ToString();
                }
            }

            return JsonConvert.SerializeObject(messageJson, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }

        private static bool IsNumeric(object value)
        {
            return value is sbyte
                || value is byte
                || value is short
                || value is ushort
                || value is int
                || value is uint
                || value is long
                || value is ulong
                || value is float
                || value is double
                || value is decimal;
        }
    }
}
