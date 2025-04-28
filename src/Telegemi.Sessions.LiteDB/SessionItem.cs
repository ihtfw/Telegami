namespace Telegami.Sessions.LiteDB;

class SessionItem
{
    public long Id { get; set; }

    public long ChatId { get; set; }
    public long UserId { get; set; }
    public int? ThreadId { get; set; }

    public TelegamiSession Data { get; set; } = new();
}