using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Telegami.Commands;
using Telegami.MessageHandlers;
using Telegram.Bot.Types.Enums;

namespace Telegami.Middlewares;

class MessagesHandler : IMessagesHandler
{
    private readonly Dictionary<int, List<IMessageHandler>> _priorityHandlers = new();

    public IReadOnlyList<IMessageHandler> Handlers { get; private set; } = Array.Empty<IMessageHandler>();

    private void Add(IMessageHandler messageHandler)
    {
        if (!_priorityHandlers.TryGetValue(messageHandler.Options.Priority, out var items))
        {
            items = new List<IMessageHandler>();
            _priorityHandlers.Add(messageHandler.Options.Priority, items);
        }

        items.Add(messageHandler);

        Handlers = _priorityHandlers
            .OrderBy(x => x.Key)
            .SelectMany(x => x.Value)
            .ToList();
    }

    public void Command<TCommandHandler>(string command, MessageHandlerOptions? options = null) where TCommandHandler : ITelegamiCommandHandler
    {
        Add(new CommandMessageHandler(command, async (MessageContext ctx) =>
        {
            var handler = ctx.Scope.ServiceProvider.GetService<TCommandHandler>();
            if (handler == null)
            {
                throw new InvalidOperationException($"ITelegamiCommandHandler of type {typeof(TCommandHandler).FullName} not registered in the service provider. Please call AddCommands to register them. Example: serviceCollection.AddTelegamiBot(\"token\").AddCommands(typeof(AssemblyMarkerType))");
            }
            
            await handler.HandleAsync(ctx);
        }, options));
    }

    /// <summary>
    /// Add a handler for the /command.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public void Command(string command, Delegate handler, MessageHandlerOptions? options = null)
    {
        Add(new CommandMessageHandler(command, handler, options));
    }

    /// <summary>
    /// Add handler when receives message with specific text.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public void Hears(string text, Delegate handler, MessageHandlerOptions? options = null)
    {
        Add(new HearsMessageHandler(text, handler, options));
    }

    /// <summary>
    /// Handle only specific message type
    /// </summary>
    /// <param name="messageType"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public void On(MessageType messageType, Delegate handler, MessageHandlerOptions? options = null)
    {
        Add(new TypeMessageHandler(messageType, handler, options));
    }

    /// <summary>
    /// Handles only CallbackQuery
    /// </summary>
    /// <param name="updateType"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public void On(UpdateType updateType, Delegate handler, MessageHandlerOptions? options = null)
    {
        Add(new TypeUpdateMessageHandler(updateType, handler, options));
    }

    /// <summary>
    /// Handles only CallbackQuery
    /// </summary>
    /// <param name="match"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public void CallbackQuery(string match, Delegate handler, MessageHandlerOptions? options = null)
    {
        Add(new CallbackQueryMessageHandler(match, handler, options));
    }

    /// <summary>
    /// Handles only CallbackQuery
    /// </summary>
    /// <param name="match"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public void CallbackQuery(Regex match, Delegate handler, MessageHandlerOptions? options = null)
    {
        Add(new CallbackQueryMessageHandler(match, handler, options));
    }
}