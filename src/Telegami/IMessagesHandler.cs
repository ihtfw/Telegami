using Telegram.Bot.Types.Enums;

namespace Telegami;

public interface IMessagesHandler
{
    internal IReadOnlyList<IMessageHandler> Handlers { get; }

    void Command(string command, Delegate handler);
    void Hears(string text, Delegate handler);
    void On(MessageType messageType, Delegate handler);
}