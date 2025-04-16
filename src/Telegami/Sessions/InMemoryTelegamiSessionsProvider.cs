using System.Collections.Concurrent;
using System.Text.Json;

namespace Telegami.Sessions;

public class InMemoryTelegamiSessionsProvider : ITelegamiSessionsProvider
{
    private readonly ConcurrentDictionary<TelegamiSessionKey, (Type type, string data)> _sessions = new();

    public Task<ITelegamiSession?> GetAsync(TelegamiSessionKey key)
    {
        if (_sessions.TryGetValue(key, out var item))
        {
            var type = item.type;
            var json = item.data;

            var jsonSession = JsonSerializer.Deserialize(json, type) as ITelegamiSession;
            if (jsonSession == null)
            {
                return Task.FromResult<ITelegamiSession?>(null);
            }

            if (jsonSession.Scenes.Count > 1)
            {
                // workaround to fix deserialization order problem
                var scenes = jsonSession.Scenes.ToArray();
                jsonSession.Scenes.Clear();
                jsonSession.Scenes.PushRange(scenes);
            }

            return Task.FromResult<ITelegamiSession?>(jsonSession);
        }

        return Task.FromResult<ITelegamiSession?>(null);
    }

    public Task SetAsync(TelegamiSessionKey key, ITelegamiSession session)
    {
        _sessions[key] = (session.GetType(), JsonSerializer.Serialize(session));
        return Task.CompletedTask;
    }
}