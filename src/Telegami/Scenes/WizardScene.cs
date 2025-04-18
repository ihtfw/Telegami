using Telegami.MessageHandlers;
using Telegami.Middlewares;

namespace Telegami.Scenes;

public class WizardScene : Scene
{
    private bool _isDefaultHandlerAdded;

    private readonly List<IMessageHandler> _stages = new();

    public WizardScene(string name, params Delegate[] stages) : base(name)
    {
        foreach (var stage in stages)
        {
            Add(stage);
        }

        this.Enter(async (MessageContext ctx, WizardContext wCtx) =>
        {
            // we add it here, so developer can add commands and other handlers before we handle the message
            AddDefaultHandler();

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

    private void AddDefaultHandler()
    {
        if (_isDefaultHandlerAdded) return;

        _isDefaultHandlerAdded = true;
        MessagesHandlerEx.On(this, async (MessageContext ctx, WizardContext wCtx) =>
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