using Blackbird.Applications.Sdk.Common;

namespace Apps.Kit.Models.Requests;

public class GetSubscriberStatsRequest
{
    [Display("Subscriber ID", Description = "Kit subscriber profile ID")]
    public string SubscriberId { get; set; } = string.Empty;

    [Display("Email sent after")]
    public DateTime? SentAfter { get; set; }

    [Display("Email sent before")]
    public DateTime? SentBefore { get; set; }
}
