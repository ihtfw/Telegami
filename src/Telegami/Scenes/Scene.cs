using Telegami.Commands;
using Telegami.MessageHandlers;
using Telegami.Middlewares;
using Telegram.Bot.Types.Enums;

namespace Telegami.Scenes
{
    public class Scene : IScene
    {
        private readonly MessagesHandler _messagesHandler = new();

        public string Name { get; }

        private IMessageHandler EnterHandler { get; set; } = EmptyMessageHandler.Instance;

        IMessageHandler IScene.EnterHandler => EnterHandler;

        private IMessageHandler LeaveHandler { get; set; } = EmptyMessageHandler.Instance;

        IMessageHandler IScene.LeaveHandler => LeaveHandler;

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

        public void Leave(Func<MessageContext, Task> func)
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

        public void Enter(Func<MessageContext, Task> func)
        {
            Enter((Delegate)func);
        }

        #region IMessagesHandler

        IReadOnlyList<IMessageHandler> IMessagesHandler.Handlers => _messagesHandler.Handlers;

        public void Command<TCommandHandler>(string command) where TCommandHandler : ITelegamiCommandHandler
        {
            _messagesHandler.Command<TCommandHandler>(command);
        }

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

        public override string ToString()
        {
            return Name;
        }
    }
}
