using LiteDB;

namespace Telegami.Sessions.LiteDB;

public class LiteDBTelegamiSessionsProviderConfig
{
    public ConnectionString ConnectionString { get; set; } = new()
    {
        Filename = "TelegamiSessions.db"
    };
}