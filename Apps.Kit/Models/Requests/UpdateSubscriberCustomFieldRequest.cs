using Blackbird.Applications.Sdk.Common;
using Apps.Kit.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Kit.Models.Requests;

public class UpdateSubscriberCustomFieldRequest
{
    [Display("Subscriber ID", Description = "Kit subscriber profile ID.")]
    public string SubscriberId { get; set; } = string.Empty;

    [Display("Custom Field", Description = "Kit custom field for a subscriber."), DataSource(typeof(CustomFieldDataHandler))]
    public string CustomField { get; set; } = string.Empty;

    [Display("Custom Field Value", Description = "Value for the selected custom field.")]
    public string Value { get; set; } = string.Empty;
}
