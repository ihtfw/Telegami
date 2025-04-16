using Telegami.MessageHandlers;
using Telegram.Bot.Types.Enums;

namespace Telegami;

class MessagesHandler : IMessagesHandler
{
    private readonly List<IMessageHandler> _handlers = new();

    public IReadOnlyList<IMessageHandler> Handlers => _handlers;

    /// <summary>
    /// Add a handler for the /command.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="handler"></param>
    public void Command(string command, Delegate handler)
    {
        _handlers.Add(new CommandMessageHandler(command, handler));
    }

    /// <summary>
    /// Add handler when receives message with specific text.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="handler"></param>
    public void Hears(string text, Delegate handler)
    {
        _handlers.Add(new HearsMessageHandler(text, handler));
    }

    /// <summary>
    /// Handle only specific message type
    /// </summary>
    /// <param name="messageType"></param>
    /// <param name="handler"></param>
    public void On(MessageType messageType, Delegate handler)
    {
        _handlers.Add(new TypeMessageHandler(messageType, handler));
    }
}