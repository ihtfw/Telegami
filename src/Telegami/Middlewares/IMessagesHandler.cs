using Telegami.Commands;
using Telegram.Bot.Types.Enums;

namespace Telegami.Middlewares;

public interface IMessagesHandler
{
    internal IReadOnlyList<IMessageHandler> Handlers { get; }
    void Command<TCommandHandler>(string command) where TCommandHandler : ITelegamiCommandHandler;
    void Command(string command, Delegate handler);
    void Hears(string text, Delegate handler);
    void On(MessageType messageType, Delegate handler);
}