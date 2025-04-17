using LiteDB;

namespace Telegami.Sessions.LiteDB;

public class LiteDBTelegamiSessionsProvider : ITelegamiSessionsProvider, IDisposable
{
    readonly LiteDatabase _liteDatabase;

    public LiteDBTelegamiSessionsProvider(LiteDBTelegamiSessionsProviderConfig config)
    {
        _liteDatabase = new LiteDatabase(config.ConnectionString);

        var liteCollection = _liteDatabase.GetCollection<SessionItem>();
        liteCollection.EnsureIndex(x => x.ChatId);
        liteCollection.EnsureIndex(x => x.ThreadId);
        liteCollection.EnsureIndex(x => x.UserId);
    }

    public Task<TelegamiSession?> GetAsync(TelegamiSessionKey key)
    {
        var collection = _liteDatabase.GetCollection<SessionItem>();

        var sessionItem = collection.FindOne(x => x.UserId == key.UserId && x.ChatId == key.ChatId && x.ThreadId == key.ThreadId);
        if (sessionItem != null)
        {
            return Task.FromResult<TelegamiSession?>(sessionItem.Data);
        }

        return Task.FromResult<TelegamiSession?>(null);
    }

    public Task SetAsync(TelegamiSessionKey key, TelegamiSession session)
    {
        var collection = _liteDatabase.GetCollection<SessionItem>();

        var sessionItem = collection.FindOne(x =>
            x.UserId == key.UserId && x.ChatId == key.ChatId && x.ThreadId == key.ThreadId);
        if (sessionItem == null)
        {
            sessionItem = new SessionItem
            {
                UserId = key.UserId,
                ChatId = key.ChatId,
                ThreadId = key.ThreadId,
                Data = session
            };

            collection.Insert(sessionItem);
            return Task.CompletedTask;
        }

        sessionItem.Data = session;
        collection.Update(sessionItem);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _liteDatabase.Dispose();
    }
}