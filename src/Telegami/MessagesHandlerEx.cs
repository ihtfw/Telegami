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
    public static T Start<T>(this T messagesHandler, Func<MessageContext, Task> handler, MessageHandlerOptions? options = null) where T : IMessagesHandler
    {
        messagesHandler.Command("start", handler, options);
        return messagesHandler;
    }

    /// <summary>
    /// Add a handler for the /start command.
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static T Start<T>(this T messagesHandler, Func<Task> handler, MessageHandlerOptions? options = null) where T : IMessagesHandler
    {
        messagesHandler.Command("start", handler, options);
        return messagesHandler;
    }

    /// <summary>
    /// Add a handler for the /start command.
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static T Start<T>(this T messagesHandler, Delegate handler, MessageHandlerOptions? options = null) where T : IMessagesHandler
    {
        messagesHandler.Command("start", handler, options);
        return messagesHandler;
    }

    /// <summary>
    /// Add a handler for the /help command.
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static T Help<T>(this T messagesHandler, Func<Task> handler, MessageHandlerOptions? options = null) where T : IMessagesHandler
    {
        messagesHandler.Command("help", handler, options);
        return messagesHandler;
    }

    /// <summary>
    /// Add a handler for the /help command.
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static T Help<T>(this T messagesHandler, Func<MessageContext, Task> handler, MessageHandlerOptions? options = null) where T : IMessagesHandler
    {
        messagesHandler.Command("help", handler, options);
        return messagesHandler;
    }

    /// <summary>
    /// Add a handler for the /help command.
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static T Help<T>(this T messagesHandler, Delegate handler, MessageHandlerOptions? options = null) where T : IMessagesHandler
    {
        messagesHandler.Command("help", handler, options);
        return messagesHandler;
    }

    /// <summary>
    /// Add a handler for the /settings command.
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static T Settings<T>(this T messagesHandler, Func<Task> handler, MessageHandlerOptions? options = null) where T : IMessagesHandler
    {
        messagesHandler.Command("settings", handler, options);
        return messagesHandler;
    }

    /// <summary>
    /// Add a handler for the /settings command.
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static T Settings<T>(this T messagesHandler, Func<MessageContext, Task> handler, MessageHandlerOptions? options = null) where T : IMessagesHandler
    {
        messagesHandler.Command("settings", handler, options);
        return messagesHandler;
    }

    /// <summary>
    /// Add a handler for the /settings command.
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static T Settings<T>(this T messagesHandler, Delegate handler, MessageHandlerOptions? options = null) where T : IMessagesHandler
    {
        messagesHandler.Command("settings", handler, options);
        return messagesHandler;
    }

    /// <summary>
    /// Add a handler for the /command.
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="command"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static T Command<T>(this T messagesHandler, string command, Func<Task> handler, MessageHandlerOptions? options = null) where T : IMessagesHandler
    {
        messagesHandler.Command(command, handler, options);
        return messagesHandler;
    }

    /// <summary>
    /// Add a handler for the /command.
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="command"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static T Command<T>(this T messagesHandler, string command, Func<MessageContext, Task> handler, MessageHandlerOptions? options = null) where T : IMessagesHandler
    {
        messagesHandler.Command(command, handler, options);
        return messagesHandler;
    }

    /// <summary>
    /// Add handler when receives message with specific text.
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="text"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static T Hears<T>(this T messagesHandler, string text, Func<Task> handler, MessageHandlerOptions? options = null) where T : IMessagesHandler
    {
        messagesHandler.Hears(text, handler, options);
        return messagesHandler;
    }

    /// <summary>
    /// Add handler when receives message with specific text.
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="text"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static T Hears<T>(this T messagesHandler, string text, Func<MessageContext, Task> handler, MessageHandlerOptions? options = null) where T : IMessagesHandler
    {
        messagesHandler.Hears(text, handler, options);
        return messagesHandler;
    }

    /// <summary>
    /// Handle only specific message type
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="messageType"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static T On<T>(this T messagesHandler, MessageType messageType, Func<Task> handler, MessageHandlerOptions? options = null) where T : IMessagesHandler
    {
        messagesHandler.On(messageType, handler, options);
        return messagesHandler;
    }

    /// <summary>
    /// Handle only specific message type
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="messageType"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static T On<T>(this T messagesHandler, MessageType messageType, Func<MessageContext, Task> handler, MessageHandlerOptions? options = null) where T : IMessagesHandler
    {
        messagesHandler.On(messageType, handler, options);
        return messagesHandler;
    }


    /// <summary>
    /// Handle only specific update type
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="updateType"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static T On<T>(this T messagesHandler, UpdateType updateType, Func<Task> handler, MessageHandlerOptions? options = null) where T : IMessagesHandler
    {
        messagesHandler.On(updateType, handler, options);
        return messagesHandler;
    }

    /// <summary>
    /// Handle only specific update type
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="updateType"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static T On<T>(this T messagesHandler, UpdateType updateType, Func<MessageContext, Task> handler, MessageHandlerOptions? options = null) where T : IMessagesHandler
    {
        messagesHandler.On(updateType, handler, options);
        return messagesHandler;
    }

    /// <summary>
    /// Handle any message
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static T On<T>(this T messagesHandler, Func<Task> handler, MessageHandlerOptions? options = null) where T : IMessagesHandler
    {
        messagesHandler.On(MessageType.Unknown, handler, options);
        return messagesHandler;
    }

    /// <summary>
    /// Handle any message
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static T On<T>(this T messagesHandler, Func<MessageContext, Task> handler, MessageHandlerOptions? options = null) where T : IMessagesHandler
    {
        messagesHandler.On(MessageType.Unknown, handler, options);
        return messagesHandler;
    }

    /// <summary>
    /// Handle any message
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static T On<T>(this T messagesHandler, Delegate handler, MessageHandlerOptions? options = null) where T : IMessagesHandler
    {
        messagesHandler.On(MessageType.Unknown, handler, options);
        return messagesHandler;
    }

    /// <summary>
    /// Handle only specific update type
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="match"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static T CallbackQuery<T>(this T messagesHandler, string match, Func<Task> handler, MessageHandlerOptions? options = null) where T : IMessagesHandler
    {
        messagesHandler.CallbackQuery(match, handler, options);
        return messagesHandler;
    }

    /// <summary>
    /// Handle only specific update type
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="match"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static T CallbackQuery<T>(this T messagesHandler, string match, Func<MessageContext, Task> handler, MessageHandlerOptions? options = null) where T : IMessagesHandler
    {
        messagesHandler.CallbackQuery(match, handler, options);
        return messagesHandler;
    }


    /// <summary>
    /// Handle only specific update type
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="match"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static T CallbackQuery<T>(this T messagesHandler, Regex match, Func<Task> handler, MessageHandlerOptions? options = null) where T : IMessagesHandler
    {
        messagesHandler.CallbackQuery(match, handler, options);
        return messagesHandler;
    }

    /// <summary>
    /// Handle only specific update type
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="match"></param>
    /// <param name="handler"></param>
    /// <param name="options"></param>
    public static T CallbackQuery<T>(this T messagesHandler, Regex match, Func<MessageContext, Task> handler, MessageHandlerOptions? options = null) where T : IMessagesHandler
    {
        messagesHandler.CallbackQuery(match, handler, options);
        return messagesHandler;
    }
}