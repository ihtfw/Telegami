using System.Collections.Concurrent;

namespace Telegami.Sessions
{
    public static class TelegamiSessionEx
    {
        public static string? Get(this ITelegamiSession telegamiSession, string key)
        {
            return telegamiSession.KeyValues.GetValueOrDefault(key);
        }

        public static void Set(this ITelegamiSession telegamiSession, string key, string value)
        {
            telegamiSession.KeyValues.AddOrUpdate(key, value, ((s, s1) => value));
        }
    }

    public interface ITelegamiSession
    {
        int Version { get; set; }
        long CreatedTimestamp { get; set; }
        long UpdatedTimestamp { get; set; }
        ConcurrentDictionary<string, string> KeyValues { get; set; }
        string? Scene { get; set; }
        string? SceneState { get; set; }
    }
}
