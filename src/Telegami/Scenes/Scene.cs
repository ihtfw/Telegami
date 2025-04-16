using Telegram.Bot.Types.Enums;

namespace Telegami.Scenes
{
    public class Scene : IScene
    {
        public string Name { get; }

        public Scene(string name)
        {
            Name = name;
        }

        public void Leave(Func<IMessageContext, Task> func)
        {

        }

        public void Enter(Func<IMessageContext, Task> func)
        {
            throw new NotImplementedException();
        }

        public void Command(string name, Func<IMessageContext, Task> func)
        {
            throw new NotImplementedException();
        }

        public void On(MessageType messageType, Func<IMessageContext, Task> func)
        {
            throw new NotImplementedException();
        }

        public void On(Func<IMessageContext, Task> func)
        {
            On(MessageType.Unknown, func);
        }
    }
}
