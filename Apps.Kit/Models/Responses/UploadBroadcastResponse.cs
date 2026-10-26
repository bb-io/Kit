using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Kit.Models.Responses;

public class UploadBroadcastResponse
{
    [Display("Broadcast")]
    public BroadcastResponse Broadcast { get; set; } = new();

    [Display("Content", Description = "Localized content with Blackbird interoperability metadata")]
    public FileReference Content { get; set; } = new();
}
