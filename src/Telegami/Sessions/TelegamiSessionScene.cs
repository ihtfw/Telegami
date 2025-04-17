namespace Telegami.Sessions;

public class TelegamiSessionScene
{
    public required string Name { get; set; }
    public int StageIndex { get; set; }

    public override string ToString()
    {
        return $"{Name} {StageIndex}";
    }
}