using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Telegami.Sessions;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class TelegamiSession : ITelegamiSession
{
    public int Version { get; set; } = 1;

    public long CreatedTimestamp { get; set; } 
    public long UpdatedTimestamp { get; set; }

    public ConcurrentDictionary<string, string> KeyValues { get; set; } = new();

    public string? Scene { get; set; }
    public string? SceneState { get; set; }
}