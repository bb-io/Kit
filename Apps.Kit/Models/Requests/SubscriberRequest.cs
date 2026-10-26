using Blackbird.Applications.Sdk.Common;

namespace Apps.Kit.Models.Requests;

public class SubscriberRequest
{
    [Display("Subscriber ID", Description = "Kit subscriber profile ID")]
    public string SubscriberId { get; set; } = string.Empty;
}
