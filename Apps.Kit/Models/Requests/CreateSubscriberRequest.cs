using Apps.Kit.DataSourceHandlers.StaticDataHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Kit.Models.Requests;

public class CreateSubscriberRequest
{
    [Display("Email address", Description = "Email address of the subscriber")]
    public string EmailAddress { get; set; } = string.Empty;

    [Display("First name", Description = "First name of the subscriber")]
    public string? FirstName { get; set; }

    [Display("Status", Description = "Create the subscriber with this status")]
    [StaticDataSource(typeof(SubscriberStateDataHandler))]
    public string? State { get; set; }
}
