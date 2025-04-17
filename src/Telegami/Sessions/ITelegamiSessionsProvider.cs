namespace Telegami.Sessions;

public interface ITelegamiSessionsProvider
{
    Task<TelegamiSession?> GetAsync(TelegamiSessionKey key);

    Task SetAsync(TelegamiSessionKey key, TelegamiSession session);
}