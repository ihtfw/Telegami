using Telegram.Bot.Types;

namespace Telegami.Sessions;

public class TelegamiSessionScene
{
    public required string Name { get; set; }
    public int StageIndex { get; set; }

    /// <summary>
    /// Messages that was sent by bot
    /// </summary>
    public HashSet<int>? BotMessageIds { get; set; }

    /// <summary>
    /// New messages that was sent by the user
    /// </summary>
    public HashSet<int>? UserMessageIds { get; set; }

    public override string ToString()
    {
        return $"{Name} {StageIndex}";
    }

    public bool AddBotMessageIds(IEnumerable<Message> messages)
    {
        var any = false;
        foreach (var message in messages)
        {
            var current = AddBotMessageId(message);
            any = any || current;
        }

        return any;
    }

    public bool AddBotMessageId(Message message) => AddBotMessageId(message.Id);
    public bool AddBotMessageId(int messageId)
    {
        if (BotMessageIds == null)
        {
            BotMessageIds = new HashSet<int>();
        }

        return BotMessageIds.Add(messageId);
    }

    public bool AddUserMessageId(int messageId)
    {
        if (UserMessageIds == null)
        {
            UserMessageIds = new HashSet<int>();
        }

        return UserMessageIds.Add(messageId);
    }
}