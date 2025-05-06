using Telegami.Scenes;

namespace Telegami;

public class WizardContext
{
    protected readonly MessageContext Ctx;

    internal WizardContext(MessageContext ctx)
    {
        Ctx = ctx;
    }

    public void ForceExecute()
    {
        Ctx.IsRetry = true;
    }

    public void Next()
    {
        var current = Ctx.Session.Scenes.LastOrDefault();
        if (current != null)
        {
            current.StageIndex++;
        }
    }

    public void Prev()
    {
        var current = Ctx.Session.Scenes.LastOrDefault();
        if (current != null)
        {
            current.StageIndex--;
        }
    }

    public void Set(string stageName)
    {
        var current = Ctx.Session.Scenes.LastOrDefault();
        if (current != null)
        {
            if (Ctx.CurrentScene is WizardScene wizardScene)
            {
                current.StageIndex = wizardScene.GetIndex(stageName);
            }
            else
            {
                current.StageIndex = -1;
            }
        }
    }

    public void Set(int index)
    {
        var current = Ctx.Session.Scenes.LastOrDefault();
        if (current != null)
        {
            current.StageIndex = index;
        }
    }

    public int Index
    {
        get
        {
            var current = Ctx.Session.Scenes.LastOrDefault();
            if (current != null)
            {
                return current.StageIndex;
            }
            return 0;
        }
    }
}