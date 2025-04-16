using Telegami.Sessions;

namespace Telegami;

public sealed class WizardContext<TState> : WizardContext, IHaveInvokeAfterEffect where TState : class, new()
{
    public TState State { get; }

    internal WizardContext(MessageContext ctx) : base(ctx)
    {
        State = ctx.Session.Get<TState>() ?? new TState();
    }

    public Task InvokeAfterEffectAsync()
    {
        Ctx.Session.Set(State);
        return Task.CompletedTask;
    }
}

public class WizardContext
{
    protected readonly MessageContext Ctx;

    internal WizardContext(MessageContext ctx)
    {
        Ctx = ctx;
    }

    public void Next()
    {
        Ctx.Session.SceneStageIndex++;
    }

    public void Prev()
    {
        Ctx.Session.SceneStageIndex--;
    }

    public void Set(int index)
    {
        Ctx.Session.SceneStageIndex = index;
    }

    public int Index => Ctx.Session.SceneStageIndex;
}