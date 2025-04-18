using System.Text.RegularExpressions;
using Telegami.Commands;
using Telegami.MessageHandlers;
using Telegram.Bot.Types.Enums;

namespace Telegami.Middlewares;

public interface IMessagesHandler
{
    internal IReadOnlyList<IMessageHandler> Handlers { get; }

    void Command<TCommandHandler>(string command, MessageHandlerOptions? options = null) where TCommandHandler : ITelegamiCommandHandler;
    void Command(string command, Delegate handler, MessageHandlerOptions? options = null);

    void Hears(string text, Delegate handler, MessageHandlerOptions? options = null);

    void On(MessageType messageType, Delegate handler, MessageHandlerOptions? options = null);

    void On(UpdateType updateType, Delegate handler, MessageHandlerOptions? options = null);

    void CallbackQuery(string match, Delegate handler, MessageHandlerOptions? options = null);

    void CallbackQuery(Regex match, Delegate handler, MessageHandlerOptions? options = null);
}