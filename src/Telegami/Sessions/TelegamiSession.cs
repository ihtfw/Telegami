using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Telegami.Sessions;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class TelegamiSession : ITelegamiSession
{
    public ConcurrentDictionary<string, string> KeyValues { get; set; } = new();

    public string? Scene { get; set; }

    public int SceneStageIndex { get; set; }

    public void Reset()
    {
        KeyValues.Clear();
        Scene = null;
        SceneStageIndex = 0;
    }
}