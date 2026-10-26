using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;
using Apps.Kit.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Kit.Models.Requests;

public class UpdateBroadcastRequest
{
    [Display("Broadcast ID"), DataSource(typeof(BroadcastDataHandler))]
    public string BroadcastId { get; set; } = string.Empty;

    [Display("Subject")]
    public string Subject { get; set; } = string.Empty;

    [Display("Content")]
    public FileReference Content { get; set; } = new();

    [Display("Preview text")]
    public string? PreviewText { get; set; }

    [Display("Description")]
    public string? Description { get; set; }
}
