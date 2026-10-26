using Apps.Kit.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Kit.Models.Requests;

public class TagSubscriberRequest
{
    [Display("Subscriber ID", Description = "Kit subscriber profile ID")]
    public string SubscriberId { get; set; } = string.Empty;

    [Display("Tag", Description = "Kit tag of the subscriber")]
    [DataSource(typeof(TagDataHandler))]
    public string TagId { get; set; } = string.Empty;
}
