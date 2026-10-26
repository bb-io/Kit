using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Kit.Models.Requests;

public class CreateBroadcastRequest
{
    [Display("Subject", Description = "Subject of the Kit broadcast")]
    public string Subject { get; set; } = string.Empty;

    [Display("Content", Description = "HTML content of the broadcast")]
    public FileReference Content { get; set; } = new();

    [Display("Preview text", Description = "Preview text of the Kit broadcast")]
    public string? PreviewText { get; set; }

    [Display("Description", Description = "Description of the Kit broadcast")]
    public string? Description { get; set; }
}
