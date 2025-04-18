using Telegami.Scenes;
using Telegami.Sessions;

namespace Telegami.Middlewares
{
    internal class MessageHandlerMiddleware : ITelegamiMiddleware
    {
        private readonly IMessagesHandler _messagesHandler;
        private readonly ScenesManager _scenesManager;

        public MessageHandlerMiddleware(IMessagesHandler messagesHandler, ScenesManager scenesManager)
        {
            _messagesHandler = messagesHandler;
            _scenesManager = scenesManager;
        }

        public async Task InvokeAsync(MessageContext ctx, MessageContextDelegate next)
        {
            if (ctx.IsHandled)
            {
                await next(ctx);
                return;
            }

            var resolvedMessagesHandler = _messagesHandler;

            var sceneName = ctx.Session.CurrentSceneName();
            while (sceneName != null)
            {
                if (_scenesManager.TryGet(sceneName, out var scene))
                {
                    resolvedMessagesHandler = scene!;
                    break;
                }

                ctx.Session.DropCurrentScene();
                sceneName = ctx.Session.CurrentSceneName();
            }
            
            foreach (var messageHandler in resolvedMessagesHandler.Handlers)
            {
                if (!await messageHandler.CanHandleAsync(ctx))
                {
                    continue;
                }

                await MessageHandlerUtils.InvokeAsync(ctx, messageHandler);
                ctx.IsHandled = true;
                break;
            }

            await next(ctx);
        }

    }
}
