using System.Text.RegularExpressions;
using Telegami.MessageHandlers;
using Telegami.Middlewares;
using Telegram.Bot.Types.Enums;

namespace Telegami;

public static class MessagesHandlerEx
{
    /// <summary>
    /// Add a handler for the /start command.
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static void Start(this IMessagesHandler messagesHandler, Func<MessageContext, Task> handler, MessageHandlerOptions? options = null)
    {
        messagesHandler.Command("start", handler, options);
    }

    /// <summary>
    /// Add a handler for the /start command.
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static void Start(this IMessagesHandler messagesHandler, Func<Task> handler, MessageHandlerOptions? options = null)
    {
        messagesHandler.Command("start", handler, options);
    }

    /// <summary>
    /// Add a handler for the /start command.
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static void Start(this IMessagesHandler messagesHandler, Delegate handler, MessageHandlerOptions? options = null)
    {
        messagesHandler.Command("start", handler, options);
    }

    /// <summary>
    /// Add a handler for the /help command.
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static void Help(this IMessagesHandler messagesHandler, Func<Task> handler, MessageHandlerOptions? options = null)
    {
        messagesHandler.Command("help", handler, options);
    }

    /// <summary>
    /// Add a handler for the /help command.
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static void Help(this IMessagesHandler messagesHandler, Func<MessageContext, Task> handler, MessageHandlerOptions? options = null)
    {
        messagesHandler.Command("help", handler, options);
    }

    /// <summary>
    /// Add a handler for the /help command.
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static void Help(this IMessagesHandler messagesHandler, Delegate handler, MessageHandlerOptions? options = null)
    {
        messagesHandler.Command("help", handler, options);
    }

    /// <summary>
    /// Add a handler for the /settings command.
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static void Settings(this IMessagesHandler messagesHandler, Func<Task> handler, MessageHandlerOptions? options = null)
    {
        messagesHandler.Command("settings", handler, options);
    }

    /// <summary>
    /// Add a handler for the /settings command.
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static void Settings(this IMessagesHandler messagesHandler, Func<MessageContext, Task> handler, MessageHandlerOptions? options = null)
    {
        messagesHandler.Command("settings", handler, options);
    }

    /// <summary>
    /// Add a handler for the /settings command.
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static void Settings(this IMessagesHandler messagesHandler, Delegate handler, MessageHandlerOptions? options = null)
    {
        messagesHandler.Command("settings", handler, options);
    }

    /// <summary>
    /// Add a handler for the /command.
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="command"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static void Command(this IMessagesHandler messagesHandler, string command, Func<Task> handler, MessageHandlerOptions? options = null)
    {
        messagesHandler.Command(command, handler, options);
    }

    /// <summary>
    /// Add a handler for the /command.
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="command"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static void Command(this IMessagesHandler messagesHandler, string command, Func<MessageContext, Task> handler, MessageHandlerOptions? options = null)
    {
        messagesHandler.Command(command, handler, options);
    }

    /// <summary>
    /// Add handler when receives message with specific text.
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="text"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static void Hears(this IMessagesHandler messagesHandler, string text, Func<Task> handler, MessageHandlerOptions? options = null)
    {
        messagesHandler.Hears(text, handler, options);
    }

    /// <summary>
    /// Add handler when receives message with specific text.
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="text"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static void Hears(this IMessagesHandler messagesHandler, string text, Func<MessageContext, Task> handler, MessageHandlerOptions? options = null)
    {
        messagesHandler.Hears(text, handler, options);
    }

    /// <summary>
    /// Handle only specific message type
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="messageType"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static void On(this IMessagesHandler messagesHandler, MessageType messageType, Func<Task> handler, MessageHandlerOptions? options = null)
    {
        messagesHandler.On(messageType, handler, options);
    }

    /// <summary>
    /// Handle only specific message type
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="messageType"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static void On(this IMessagesHandler messagesHandler, MessageType messageType, Func<MessageContext, Task> handler, MessageHandlerOptions? options = null)
    {
        messagesHandler.On(messageType, handler, options);
    }


    /// <summary>
    /// Handle only specific update type
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="updateType"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static void On(this IMessagesHandler messagesHandler, UpdateType updateType, Func<Task> handler, MessageHandlerOptions? options = null)
    {
        messagesHandler.On(updateType, handler, options);
    }

    /// <summary>
    /// Handle only specific update type
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="updateType"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static void On(this IMessagesHandler messagesHandler, UpdateType updateType, Func<MessageContext, Task> handler, MessageHandlerOptions? options = null)
    {
        messagesHandler.On(updateType, handler, options);
    }

    /// <summary>
    /// Handle any message
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static void On(this IMessagesHandler messagesHandler, Func<Task> handler, MessageHandlerOptions? options = null)
    {
        messagesHandler.On(MessageType.Unknown, handler, options);
    }

    /// <summary>
    /// Handle any message
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static void On(this IMessagesHandler messagesHandler, Func<MessageContext, Task> handler, MessageHandlerOptions? options = null)
    {
        messagesHandler.On(MessageType.Unknown, handler, options);
    }

    /// <summary>
    /// Handle any message
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static void On(this IMessagesHandler messagesHandler, Delegate handler, MessageHandlerOptions? options = null)
    {
        messagesHandler.On(MessageType.Unknown, handler, options);
    }

    /// <summary>
    /// Handle only specific update type
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="match"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static void CallbackQuery(this IMessagesHandler messagesHandler, string match, Func<Task> handler, MessageHandlerOptions? options = null)
    {
        messagesHandler.CallbackQuery(match, handler, options);
    }

    /// <summary>
    /// Handle only specific update type
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="match"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static void CallbackQuery(this IMessagesHandler messagesHandler, string match, Func<MessageContext, Task> handler, MessageHandlerOptions? options = null)
    {
        messagesHandler.CallbackQuery(match, handler, options);
    }


    /// <summary>
    /// Handle only specific update type
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="match"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static void CallbackQuery(this IMessagesHandler messagesHandler, Regex match, Func<Task> handler, MessageHandlerOptions? options = null)
    {
        messagesHandler.CallbackQuery(match, handler, options);
    }

    /// <summary>
    /// Handle only specific update type
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="match"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static void CallbackQuery(this IMessagesHandler messagesHandler, Regex match, Func<MessageContext, Task> handler, MessageHandlerOptions? options = null)
    {
        messagesHandler.CallbackQuery(match, handler, options);
    }
}