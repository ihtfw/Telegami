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

            do
            {
                ctx.IsRetry = false;
                ctx.IsHandled = false;

                var resolvedMessagesHandler = _messagesHandler;

                var sceneName = ctx.Session.CurrentSceneName();
                while (sceneName != null)
                {
                    if (_scenesManager.TryGet(sceneName, ctx.Scope.ServiceProvider, out var scene))
                    {
                        resolvedMessagesHandler = scene!;
                        ctx.CurrentScene = scene;
                        break;
                    }

                    ctx.Session.DropCurrentScene();
                    sceneName = ctx.Session.CurrentSceneName();
                    ctx.CurrentScene = null;
                }
                
                foreach (var messageHandler in resolvedMessagesHandler.Handlers)
                {
                    if (!await messageHandler.CanHandleAsync(ctx))
                    {
                        continue;
                    }

                    await MessageHandlerUtils.InvokeAsync(ctx, messageHandler);
                    if (!messageHandler.Options.PreventRemoveMarkup)
                    {
                        await ctx.RemoveMarkup();
                    }

                    if (messageHandler.Options.UserHandling)
                    {
                        continue;
                    }

                    ctx.IsHandled = true;
                    break;
                }

            } while (ctx.IsRetry);

            await next(ctx);
        }

    }
}
