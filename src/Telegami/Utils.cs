using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Telegami
{
    public static class Utils
    {
        public static string ToJsonDebug(object obj)
        {
            var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            });

            return json;
        }
    }
}
