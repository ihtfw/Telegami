using System.Text.RegularExpressions;
using Telegami.Commands;
using Telegami.Extensions;
using Telegami.MessageHandlers;
using Telegami.Middlewares;
using Telegram.Bot.Types.Enums;

namespace Telegami.Scenes
{
    public class Scene : IScene
    {
        private readonly MessagesHandler _messagesHandler = new();
        
        public Scene() { }

        [Obsolete("Name is not needed anymore for Scene! Just use ctor without name!", true)]
        // ReSharper disable once UnusedParameter.Local
        public Scene(string name)
        {
        }

        private IReadOnlyCollection<IMessageHandler> EnterHandlers { get; set; } = Array.Empty<IMessageHandler>();
        private IReadOnlyCollection<IMessageHandler> ReEnterHandlers { get; set; } = Array.Empty<IMessageHandler>();

        IReadOnlyCollection<IMessageHandler> IScene.EnterHandlers => EnterHandlers;
        IReadOnlyCollection<IMessageHandler> IScene.ReEnterHandlers => ReEnterHandlers;

        private IReadOnlyCollection<IMessageHandler> LeaveHandlers { get; set; } = Array.Empty<IMessageHandler>();

        IReadOnlyCollection<IMessageHandler> IScene.LeaveHandlers => LeaveHandlers;

        public void Leave(Delegate handler)
        {
            LeaveHandlers = LeaveHandlers.Add(new DelegateMessageHandler(handler));
        }

        /// <summary>
        /// Invoked on Scene Enter via EnterSceneAsync() method
        /// </summary>
        /// <param name="handler"></param>
        public void Enter(Delegate handler)
        {
            EnterHandlers = EnterHandlers.Add(new DelegateMessageHandler(handler));
        }
        
        /// <summary>
        /// Invoked after returning back from sub scene
        /// </summary>
        /// <param name="handler"></param>
        public void ReEnter(Delegate handler)
        {
            ReEnterHandlers = ReEnterHandlers.Add(new DelegateMessageHandler(handler));
        }
        
        #region IMessagesHandler

        IReadOnlyList<IMessageHandler> IMessagesHandler.Handlers => _messagesHandler.Handlers;

        public void Command<TCommandHandler>(string command, MessageHandlerOptions? options = null) where TCommandHandler : ITelegamiCommandHandler
        {
            _messagesHandler.Command<TCommandHandler>(command, options);
        }

        public void Command(string command, Delegate handler, MessageHandlerOptions? options = null)
        {
            _messagesHandler.Command(command, handler, options);
        }

        public void Hears(string text, Delegate handler, MessageHandlerOptions? options = null)
        {
            _messagesHandler.Hears(text, handler, options);
        }

        public void On(MessageType messageType, Delegate handler, MessageHandlerOptions? options = null)
        {
            _messagesHandler.On(messageType, handler, options);
        }

        public void On(UpdateType updateType, Delegate handler, MessageHandlerOptions? options = null)
        {
            _messagesHandler.On(updateType, handler, options);
        }
        
        public void CallbackQuery(string match, Delegate handler, MessageHandlerOptions? options = null)
        {
            _messagesHandler.CallbackQuery(match, handler, options);
        }

        public void CallbackQuery(Regex match, Delegate handler, MessageHandlerOptions? options = null)
        {
            _messagesHandler.CallbackQuery(match, handler, options);
        }

        #endregion
    }
}
