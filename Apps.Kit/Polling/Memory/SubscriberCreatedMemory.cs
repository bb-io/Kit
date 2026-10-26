namespace Apps.Kit.Polling.Memory;

public class SubscriberCreatedMemory
{
    public DateTime LastInteractionDate { get; set; }

    public Dictionary<string, DateTime> KnownSubscriberIds { get; set; }
        = new();
}
