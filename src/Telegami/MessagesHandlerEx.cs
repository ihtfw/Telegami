using Telegram.Bot.Types.Enums;

namespace Telegami;

public static class MessagesHandlerEx
{
    public static void Start(this IMessagesHandler messagesHandler, Func<IMessageContext, Task> handler)
    {
        messagesHandler.Command("start", handler);
    }
    public static void Start(this IMessagesHandler messagesHandler, Func<Task> handler)
    {
        messagesHandler.Command("start", handler);
    }

    public static void Start(this IMessagesHandler messagesHandler, Delegate handler)
    {
        messagesHandler.Command("start", handler);
    }

    public static void Help(this IMessagesHandler messagesHandler, Func<Task> handler)
    {
        messagesHandler.Command("help", handler);
    }
    public static void Help(this IMessagesHandler messagesHandler, Func<IMessageContext, Task> handler)
    {
        messagesHandler.Command("help", handler);
    }

    public static void Help(this IMessagesHandler messagesHandler, Delegate handler)
    {
        messagesHandler.Command("help", handler);
    }

    public static void Settings(this IMessagesHandler messagesHandler, Func<Task> handler)
    {
        messagesHandler.Command("settings", handler);
    }

    public static void Settings(this IMessagesHandler messagesHandler, Func<IMessageContext, Task> handler)
    {
        messagesHandler.Command("settings", handler);
    }

    public static void Settings(this IMessagesHandler messagesHandler, Delegate handler)
    {
        messagesHandler.Command("settings", handler);
    }

    public static void Command(this IMessagesHandler messagesHandler, string command, Func<Task> handler)
    {
        messagesHandler.Command(command, handler);
    }

    public static void Command(this IMessagesHandler messagesHandler, string command, Func<IMessageContext, Task> handler)
    {
        messagesHandler.Command(command, handler);
    }

    public static void Hears(this IMessagesHandler messagesHandler, string text, Func<Task> handler)
    {
        messagesHandler.Hears(text, handler);
    }

    public static void Hears(this IMessagesHandler messagesHandler, string text, Func<IMessageContext, Task> handler)
    {
        messagesHandler.Hears(text, handler);
    }

    public static void On(this IMessagesHandler messagesHandler, MessageType messageType, Func<Task> handler)
    {
        messagesHandler.On(messageType, handler);
    }

    public static void On(this IMessagesHandler messagesHandler, MessageType messageType, Func<IMessageContext, Task> handler)
    {
        messagesHandler.On(messageType, handler);
    }

    public static void On(this IMessagesHandler messagesHandler, Func<Task> handler)
    {
        messagesHandler.On(MessageType.Unknown, handler);
    }

    public static void On(this IMessagesHandler messagesHandler, Func<IMessageContext, Task> handler)
    {
        messagesHandler.On(MessageType.Unknown, handler);
    }

    public static void On(this IMessagesHandler messagesHandler, Delegate handler)
    {
        messagesHandler.On(MessageType.Unknown, handler);
    }
}