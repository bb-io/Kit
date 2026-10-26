namespace Apps.Kit.Polling.Memory;

public class SubscriberUnsubscribedMemory
{
    public DateTime LastInteractionDate { get; set; }

    public Dictionary<string, DateTime> KnownSubscriberIds { get; set; } = new();
}
