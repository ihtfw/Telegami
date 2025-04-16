using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Telegami.Sessions;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class TelegamiSession : ITelegamiSession
{
    public ConcurrentDictionary<string, string> KeyValues { get; set; } = new();
    public ConcurrentStack<TelegamiSessionScene> Scenes { get; set; } = new();

    public void Reset()
    {
        KeyValues.Clear();
        Scenes.Clear();
    }
}