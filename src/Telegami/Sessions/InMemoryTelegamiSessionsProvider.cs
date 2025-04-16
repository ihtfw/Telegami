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

            return Task.FromResult(jsonSession);
        }

        return Task.FromResult<ITelegamiSession?>(null);
    }

    public Task SetAsync(TelegamiSessionKey key, ITelegamiSession session)
    {
        _sessions[key] = (session.GetType(), JsonSerializer.Serialize(session));
        return Task.CompletedTask;
    }
}