using Telegami.Middlewares;

namespace Telegami.Scenes;

public class WizardScene : Scene
{
    private readonly List<IMessageHandler> _stages = new();

    public WizardScene(string name, params Delegate[] stages) : base(name)
    {
        foreach (var stage in stages)
        {
            _stages.Add(new DelegateMessageHandler(stage));
        }

        this.Enter(async (MessageContext ctx, WizardContext wCtx) =>
        {
            wCtx.Set(0);

            var messageHandler = _stages.ElementAtOrDefault(wCtx.Index);
            if (messageHandler == null)
            {
                await ctx.LeaveSceneAsync();
                return;
            }

            await MessageHandlerUtils.InvokeAsync(ctx, messageHandler);
        });

        this.On(async (MessageContext ctx, WizardContext wCtx) =>
        {
            var messageHandler = _stages.ElementAtOrDefault(wCtx.Index);
            if (messageHandler == null)
            {
                await ctx.LeaveSceneAsync();
                return;
            }

            await MessageHandlerUtils.InvokeAsync(ctx, messageHandler);
        });
    }
}