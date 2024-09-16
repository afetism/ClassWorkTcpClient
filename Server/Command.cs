namespace Server;

public class Command
{
    public string ProcessName { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Context { get; set; } = null!;

    public bool Refresh { get; set; }
}
