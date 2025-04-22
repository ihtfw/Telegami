using System.Text.Json;
using System.Text.Json.Serialization;

namespace Telegami.Example.Advanced
{
    internal static class Utils
    {
        public static string ToJson(object obj)
        {
            var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true
            });

            return json;
        }
    }
}
