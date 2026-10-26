using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Kit.Models.Requests;

public class CreateLocalizedBroadcastRequest
{
    [Display("Translated content",
        Description = "Translated broadcast HTML file.")]
    public FileReference Content { get; set; } = new();

    [Display("Target language",
        Description = "Language code for the localized broadcast, such as de-DE.")]
    public string TargetLanguage { get; set; } = string.Empty;
}
