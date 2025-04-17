namespace Telegami;

public class WizardContext
{
    protected readonly MessageContext Ctx;

    internal WizardContext(MessageContext ctx)
    {
        Ctx = ctx;
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