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
    public static void Start(this IMessagesHandler messagesHandler, Func<MessageContext, Task> handler)
    {
        messagesHandler.Command("start", handler);
    }

    /// <summary>
    /// Add a handler for the /start command.
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="handler"></param>
    public static void Start(this IMessagesHandler messagesHandler, Func<Task> handler)
    {
        messagesHandler.Command("start", handler);
    }

    /// <summary>
    /// Add a handler for the /start command.
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="handler"></param>
    public static void Start(this IMessagesHandler messagesHandler, Delegate handler)
    {
        messagesHandler.Command("start", handler);
    }

    /// <summary>
    /// Add a handler for the /help command.
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="handler"></param>
    public static void Help(this IMessagesHandler messagesHandler, Func<Task> handler)
    {
        messagesHandler.Command("help", handler);
    }

    /// <summary>
    /// Add a handler for the /help command.
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="handler"></param>
    public static void Help(this IMessagesHandler messagesHandler, Func<MessageContext, Task> handler)
    {
        messagesHandler.Command("help", handler);
    }

    /// <summary>
    /// Add a handler for the /help command.
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="handler"></param>
    public static void Help(this IMessagesHandler messagesHandler, Delegate handler)
    {
        messagesHandler.Command("help", handler);
    }

    /// <summary>
    /// Add a handler for the /settings command.
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="handler"></param>
    public static void Settings(this IMessagesHandler messagesHandler, Func<Task> handler)
    {
        messagesHandler.Command("settings", handler);
    }

    /// <summary>
    /// Add a handler for the /settings command.
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="handler"></param>
    public static void Settings(this IMessagesHandler messagesHandler, Func<MessageContext, Task> handler)
    {
        messagesHandler.Command("settings", handler);
    }

    /// <summary>
    /// Add a handler for the /settings command.
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="handler"></param>
    public static void Settings(this IMessagesHandler messagesHandler, Delegate handler)
    {
        messagesHandler.Command("settings", handler);
    }

    /// <summary>
    /// Add a handler for the /command.
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="command"></param>
    /// <param name="handler"></param>
    public static void Command(this IMessagesHandler messagesHandler, string command, Func<Task> handler)
    {
        messagesHandler.Command(command, handler);
    }

    /// <summary>
    /// Add a handler for the /command.
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="command"></param>
    /// <param name="handler"></param>
    public static void Command(this IMessagesHandler messagesHandler, string command, Func<MessageContext, Task> handler)
    {
        messagesHandler.Command(command, handler);
    }

    /// <summary>
    /// Add handler when receives message with specific text.
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="text"></param>
    /// <param name="handler"></param>
    public static void Hears(this IMessagesHandler messagesHandler, string text, Func<Task> handler)
    {
        messagesHandler.Hears(text, handler);
    }

    /// <summary>
    /// Add handler when receives message with specific text.
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="text"></param>
    /// <param name="handler"></param>
    public static void Hears(this IMessagesHandler messagesHandler, string text, Func<MessageContext, Task> handler)
    {
        messagesHandler.Hears(text, handler);
    }

    /// <summary>
    /// Handle only specific message type
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="messageType"></param>
    /// <param name="handler"></param>
    public static void On(this IMessagesHandler messagesHandler, MessageType messageType, Func<Task> handler)
    {
        messagesHandler.On(messageType, handler);
    }

    /// <summary>
    /// Handle only specific message type
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="messageType"></param>
    /// <param name="handler"></param>
    public static void On(this IMessagesHandler messagesHandler, MessageType messageType, Func<MessageContext, Task> handler)
    {
        messagesHandler.On(messageType, handler);
    }

    /// <summary>
    /// Handle any message
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="handler"></param>
    public static void On(this IMessagesHandler messagesHandler, Func<Task> handler)
    {
        messagesHandler.On(MessageType.Unknown, handler);
    }

    /// <summary>
    /// Handle any message
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="handler"></param>
    public static void On(this IMessagesHandler messagesHandler, Func<MessageContext, Task> handler)
    {
        messagesHandler.On(MessageType.Unknown, handler);
    }

    /// <summary>
    /// Handle any message
    /// </summary>
    /// <param name="messagesHandler"></param>
    /// <param name="handler"></param>
    public static void On(this IMessagesHandler messagesHandler, Delegate handler)
    {
        messagesHandler.On(MessageType.Unknown, handler);
    }
}