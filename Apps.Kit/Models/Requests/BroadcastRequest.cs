using Apps.Kit.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Kit.Models.Requests;

public class BroadcastRequest
{
    [Display("Broadcast ID", Description = "Select a Kit broadcast or map its ID from a previous step")]
    [DataSource(typeof(BroadcastDataHandler))]
    public string BroadcastId { get; set; } = string.Empty;
}
