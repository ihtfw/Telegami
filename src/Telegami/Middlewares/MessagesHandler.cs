using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Telegami.Commands;
using Telegami.MessageHandlers;
using Telegram.Bot.Types.Enums;

namespace Telegami.Middlewares;

class MessagesHandler : IMessagesHandler
{
    private readonly List<IMessageHandler> _handlers = new();

    public IReadOnlyList<IMessageHandler> Handlers => _handlers;
    
    public void Command<TCommandHandler>(string command, MessageHandlerOptions? options = null) where TCommandHandler : ITelegamiCommandHandler
    {
        _handlers.Add(new CommandMessageHandler(command, async (MessageContext ctx) =>
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
        _handlers.Add(new CommandMessageHandler(command, handler, options));
    }

    /// <summary>
    /// Add handler when receives message with specific text.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public void Hears(string text, Delegate handler, MessageHandlerOptions? options = null)
    {
        _handlers.Add(new HearsMessageHandler(text, handler, options));
    }

    /// <summary>
    /// Handle only specific message type
    /// </summary>
    /// <param name="messageType"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public void On(MessageType messageType, Delegate handler, MessageHandlerOptions? options = null)
    {
        _handlers.Add(new TypeMessageHandler(messageType, handler, options));
    }

    /// <summary>
    /// Handles only CallbackQuery
    /// </summary>
    /// <param name="updateType"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public void On(UpdateType updateType, Delegate handler, MessageHandlerOptions? options = null)
    {
        _handlers.Add(new TypeUpdateMessageHandler(updateType, handler, options));
    }

    /// <summary>
    /// Handles only CallbackQuery
    /// </summary>
    /// <param name="match"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public void CallbackQuery(string match, Delegate handler, MessageHandlerOptions? options = null)
    {
        _handlers.Add(new CallbackQueryMessageHandler(match, handler, options));
    }

    /// <summary>
    /// Handles only CallbackQuery
    /// </summary>
    /// <param name="match"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public void CallbackQuery(Regex match, Delegate handler, MessageHandlerOptions? options = null)
    {
        _handlers.Add(new CallbackQueryMessageHandler(match, handler, options));
    }
}