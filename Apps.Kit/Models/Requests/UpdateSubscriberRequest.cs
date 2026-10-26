using Blackbird.Applications.Sdk.Common;

namespace Apps.Kit.Models.Requests;

public class UpdateSubscriberRequest
{
    [Display("Subscriber ID", Description = "Kit subscriber profile ID.")]
    public string SubscriberId { get; set; } = string.Empty;

    [Display("Email Address", Description = "Email address of the subscriber.")]
    public string? EmailAddress { get; set; }

    [Display("First Name", Description = "First name of the subscriber.")]
    public string? FirstName { get; set; }
}
