using Telegami.MessageHandlers;
using Telegram.Bot.Types.Enums;

namespace Telegami;

class MessagesHandler : IMessagesHandler
{
    private readonly List<IMessageHandler> _handlers = new();

    public IReadOnlyList<IMessageHandler> Handlers => _handlers;

    public void Command(string command, Delegate handler)
    {
        _handlers.Add(new CommandMessageHandler(command, handler));
    }

    public void Hears(string text, Delegate handler)
    {
        _handlers.Add(new HearsMessageHandler(text, handler));
    }
    
    public void On(MessageType messageType, Delegate handler)
    {
        _handlers.Add(new TypeMessageHandler(messageType, handler));
    }
}