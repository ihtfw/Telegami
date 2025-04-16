using System.Collections.Concurrent;

namespace Telegami.Sessions
{
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
