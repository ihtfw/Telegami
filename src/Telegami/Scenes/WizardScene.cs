using Telegami.MessageHandlers;
using Telegami.Middlewares;

namespace Telegami.Scenes;

public class WizardScene : Scene
{
    private readonly List<IMessageHandler> _stages = new();

    public WizardScene(string name, params Delegate[] stages) : base(name)
    {
        foreach (var stage in stages)
        {
            Add(stage);
        }

        this.On(async (MessageContext ctx, WizardContext wCtx) =>
        {
            var messageHandler = _stages.ElementAtOrDefault(wCtx.Index);
            if (messageHandler == null)
            {
                await ctx.LeaveSceneAsync();
                return;
            }

            await MessageHandlerUtils.InvokeAsync(ctx, messageHandler);
        }, MessageHandlerOptions.LowPriority);

        Enter(async (MessageContext ctx, WizardContext wCtx) =>
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
    }

    public void Add<TState>(Func<MessageContext, WizardContext<TState>, Task> stage) where TState : class, new()
    {
        _stages.Add(new DelegateMessageHandler(stage));
    }

    public void Add(Func<MessageContext, WizardContext, Task> stage)
    {
        _stages.Add(new DelegateMessageHandler(stage));
    }

    public void Add(Func<MessageContext, Task> stage)
    {
        _stages.Add(new DelegateMessageHandler(stage));
    }

    public void Add(Delegate stage)
    {
        _stages.Add(new DelegateMessageHandler(stage));
    }
}