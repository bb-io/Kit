namespace Apps.Kit.Polling.Memory;

public class BroadcastPublishedMemory
{
    public DateTime LastInteractionDate { get; set; }

    public Dictionary<string, DateTime> KnownBroadcastIds { get; set; } = new();
}
