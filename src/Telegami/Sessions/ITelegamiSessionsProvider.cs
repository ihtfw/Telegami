namespace Telegami.Sessions;

public interface ITelegamiSessionsProvider
{
    Task<ITelegamiSession?> GetAsync(TelegamiSessionKey key);

    Task SetAsync(TelegamiSessionKey key, ITelegamiSession session);
}