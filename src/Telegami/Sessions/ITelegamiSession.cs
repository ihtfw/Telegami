using System.Collections.Concurrent;
using System.Text.Json;

namespace Telegami.Sessions
{
    public static class TelegamiSessionEx
    {
        public static string? Get(this ITelegamiSession telegamiSession, string key)
        {
            return telegamiSession.KeyValues.GetValueOrDefault(key);
        }

        public static void Set<T>(this ITelegamiSession telegamiSession, string key, T value) where T : class
        {
            if (value is string strValue)
            {
                telegamiSession.KeyValues.AddOrUpdate(key, strValue, ((s, s1) => strValue));
                return;
            }

            var json = JsonSerializer.Serialize(value);
            telegamiSession.KeyValues.AddOrUpdate(key, json, ((s, s1) => json));
        }

        public static void Set<T>(this ITelegamiSession telegamiSession, T value) where T : class
        {
            var key = typeof(T).FullName ?? typeof(T).Name;
            Set(telegamiSession, key, value);
        }

        public static T? Get<T>(this ITelegamiSession telegamiSession) where T : class
        {
            var key = typeof(T).FullName ?? typeof(T).Name;
            return telegamiSession.Get<T>(key);
        }

        public static T? Get<T>(this ITelegamiSession telegamiSession, string key) where T : class
        {
            if (!telegamiSession.KeyValues.TryGetValue(key, out var value))
            {
                return null;
            }

            if (typeof(T) == typeof(string))
            {
                return value as T;
            }

            return JsonSerializer.Deserialize<T>(value);
        }


        public static void Set(this ITelegamiSession telegamiSession, string key, string value)
        {
            telegamiSession.KeyValues.AddOrUpdate(key, value, ((s, s1) => value));
        }
    }

    public interface ITelegamiSession
    {
        ConcurrentDictionary<string, string> KeyValues { get; set; }
        string? Scene { get; set; }
        int SceneStageIndex { get; set; }
        void Reset();
    }
}
