using Apps.Kit.DataSourceHandlers.StaticDataHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Kit.Models.Requests;

public class SearchBroadcastsRequest
{
    [Display("Status")]
    [StaticDataSource(typeof(BroadcastStatusDataHandler))]
    public string? Status { get; set; }

    [Display("Sent after")]
    public DateTime? SentAfter { get; set; }

    [Display("Sent before")]
    public DateTime? SentBefore { get; set; }
}
