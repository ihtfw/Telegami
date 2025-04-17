using System.Collections.Concurrent;

namespace Telegami.Sessions
{
    public interface ITelegamiSession
    {
        ConcurrentDictionary<string, string> KeyValues { get; set; }
        ConcurrentStack<TelegamiSessionScene> Scenes { get; set; }

        void Reset();
    }
}
