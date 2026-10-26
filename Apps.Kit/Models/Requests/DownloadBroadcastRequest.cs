using Apps.Kit.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Kit.Models.Requests;

public class DownloadBroadcastRequest : IDownloadContentInput
{
    [Display("Broadcast ID")]
    [DataSource(typeof(BroadcastDataHandler))]
    public string ContentId { get; set; } = string.Empty;

    [Display("Source language", Description = "Language code of the broadcast content, such as en or en-US")]
    public string SourceLanguage { get; set; } = string.Empty;
}
