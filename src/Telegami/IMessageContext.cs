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
        if (Ctx.Session.Scenes.TryPeek(out var scene))
        {
            scene.StageIndex++;
        }
    }

    public void Prev()
    {
        if (Ctx.Session.Scenes.TryPeek(out var scene))
        {
            scene.StageIndex--;
        }
    }

    public void Set(int index)
    {
        if (Ctx.Session.Scenes.TryPeek(out var scene))
        {
            scene.StageIndex = index;
        }
    }

    public int Index
    {
        get
        {
            if (Ctx.Session.Scenes.TryPeek(out var scene))
            {
                return scene.StageIndex;
            }
            return 0;
        }
    }
}