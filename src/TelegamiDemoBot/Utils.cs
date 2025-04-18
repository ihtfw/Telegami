using System.Text.Json.Serialization;
using System.Text.Json;

namespace TelegamiDemoBot
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
