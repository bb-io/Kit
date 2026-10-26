using Blackbird.Applications.Sdk.Common;
using Apps.Kit.DataSourceHandlers.EnumDataHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Kit.Models.Requests;

public class CreateSubscriberRequest
{
    [Display("Email Address", Description = "Email address of the subscriber.")]
    public string EmailAddress { get; set; } = string.Empty;

    [Display("First Name", Description = "First name of the subscriber.")]
    public string? FirstName { get; set; }

    [Display("Status", Description = "Create the subscriber with this status."),
     StaticDataSource(typeof(SubscriberStateDataHandler))]
    public string? State { get; set; }
}
