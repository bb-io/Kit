using Blackbird.Applications.Sdk.Common;
using Apps.Kit.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Kit.Models.Requests;

public class DownloadBroadcastRequest
{
    [Display("Broadcast ID"), DataSource(typeof(BroadcastDataHandler))]
    public string BroadcastId { get; set; } = string.Empty;

    [Display("Source language")]
    public string SourceLanguage { get; set; } = string.Empty;
}
