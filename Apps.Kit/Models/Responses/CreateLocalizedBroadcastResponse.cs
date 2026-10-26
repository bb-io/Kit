using Apps.Kit.Models.Dtos;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Kit.Models.Responses;

public class CreateLocalizedBroadcastResponse
{
    [Display("Broadcast")]
    public BroadcastDto Broadcast { get; set; } = new();

    [Display("Content", Description = "Localized content with Blackbird interoperability metadata")]
    public FileReference? Content { get; set; }

    [Display("Target language")]
    public string TargetLanguage { get; set; } = string.Empty;
}
