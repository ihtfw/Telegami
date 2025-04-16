using Telegram.Bot.Types.Enums;

namespace Telegami.Scenes
{
    public class Scene : IScene
    {
        private readonly MessagesHandler _messagesHandler = new();

        public string Name { get; }

        public IMessageHandler EnterHandler { get; private set; } = EmptyMessageHandler.Instance;
        public IMessageHandler LeaveHandler { get; private set; } = EmptyMessageHandler.Instance;

        public Scene(string name)
        {
            Name = name;
        }

        public void Leave(Delegate handler)
        {
            LeaveHandler = new DelegateMessageHandler(handler);
        }

        public void Leave(Func<Task> func)
        {
            Leave((Delegate)func);
        }

        public void Leave(Func<IMessageContext, Task> func)
        {
            Leave((Delegate)func);
        }

        public void Enter(Delegate handler)
        {
            EnterHandler = new DelegateMessageHandler(handler);
        }

        public void Enter(Func<Task> func)
        {
            Enter((Delegate)func);
        }

        public void Enter(Func<IMessageContext, Task> func)
        {
            Enter((Delegate)func);
        }

        #region IMessagesHandler

        public IReadOnlyList<IMessageHandler> Handlers => _messagesHandler.Handlers;

        public void Command(string command, Delegate handler)
        {
            _messagesHandler.Command(command, handler);
        }

        public void Hears(string text, Delegate handler)
        {
            _messagesHandler.Hears(text, handler);
        }

        public void On(MessageType messageType, Delegate handler)
        {
            _messagesHandler.On(messageType, handler);
        }

        #endregion
    }
}
