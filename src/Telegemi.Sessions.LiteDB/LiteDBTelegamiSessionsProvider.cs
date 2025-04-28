using LiteDB;

namespace Telegami.Sessions.LiteDB;

public class LiteDBTelegamiSessionsProvider : ReaderWriterLockTelegamiSessionsProvider, IDisposable
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

    protected override Task<TelegamiSession?> InternalGetAsync(TelegamiSessionKey key)
    {
        var collection = _liteDatabase.GetCollection<SessionItem>();

        var sessionItem = collection.FindOne(x => x.UserId == key.UserId && x.ChatId == key.ChatId && x.ThreadId == key.ThreadId);
        if (sessionItem != null)
        {
            return Task.FromResult<TelegamiSession?>(sessionItem.Data);
        }

        return Task.FromResult<TelegamiSession?>(null);
    }

    protected override Task InternalSetAsync(TelegamiSessionKey key, TelegamiSession session)
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