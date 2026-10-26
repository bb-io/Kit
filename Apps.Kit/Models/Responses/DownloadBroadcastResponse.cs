using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Kit.Models.Responses;

public class DownloadBroadcastResponse : IDownloadContentOutput
{
    [Display("Content", Description = "Broadcast HTML with Blackbird interoperability metadata")]
    public FileReference Content { get; set; } = new();
}
