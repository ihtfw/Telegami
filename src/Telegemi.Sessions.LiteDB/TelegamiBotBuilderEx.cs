using Microsoft.Extensions.DependencyInjection;
using Telegami.Extensions;

namespace Telegami.Sessions.LiteDB;

public static class TelegamiBotBuilderEx
{
    /// <summary>
    /// To store user sessions in LiteDB, instead of default in-memory storage.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static TelegamiBotBuilder AddLiteDBSessions(this TelegamiBotBuilder builder, Action<LiteDBTelegamiSessionsProviderConfig>? configure = null)
    {
        builder.ServiceCollection.AddKeyedSingleton<ITelegamiSessionsProvider, LiteDBTelegamiSessionsProvider>(builder.Key, (
            (_, _) =>
            {
                var config = new LiteDBTelegamiSessionsProviderConfig();
                configure?.Invoke(config);

                return new LiteDBTelegamiSessionsProvider(config);
            }));

        return builder;
    }
}