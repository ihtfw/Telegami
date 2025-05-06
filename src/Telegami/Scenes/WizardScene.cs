using Telegami.MessageHandlers;
using Telegami.Middlewares;

namespace Telegami.Scenes;

public class WizardScene : Scene
{
    private readonly List<IMessageHandler> _stages = new();
    private readonly Dictionary<string, int> _stageNameToIndex = new();
    
    [Obsolete("Name is not needed anymore for WizardScene! Just use ctor without name!", true)]
    // ReSharper disable once UnusedParameter.Local
    public WizardScene(string name, params Delegate[] stages) : this(stages)
    {
    }

    public WizardScene(params Delegate[] stages)
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

        ReEnter(async (MessageContext ctx, WizardContext wCtx) =>
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

    public WizardScene Add<TState>(Func<MessageContext, WizardContext<TState>, Task> stage, string? stageName = null) where TState : class, new()
    {
        AddState(stage, stageName);
        return this;
    }

    public WizardScene Add(Func<MessageContext, WizardContext, Task> stage, string? stageName = null)
    {
        AddState(stage, stageName);
        return this;
    }

    public WizardScene Add(Func<MessageContext, Task> stage, string? stageName = null)
    {
        AddState(stage, stageName);
        return this;
    }

    public WizardScene Add(Delegate stage, string? stageName = null)
    {
        AddState(stage, stageName);
        return this;
    }

    private void AddState(Delegate stage, string? stageName)
    {
        if (!string.IsNullOrWhiteSpace(stageName))
        {
            _stageNameToIndex[stageName] = _stages.Count;
        }
        _stages.Add(new DelegateMessageHandler(stage));
    }

    public int GetIndex(string stageName)
    {
        if (_stageNameToIndex.TryGetValue(stageName, out var index))
        {
            return index;
        }

        return -1;
    }
}