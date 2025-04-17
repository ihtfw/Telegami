using Telegami.Sessions;

namespace Telegami.Scenes;

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