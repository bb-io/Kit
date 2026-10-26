using Apps.Kit.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Kit.Models.Requests;

public class UploadBroadcastRequest : IUploadContentInput
{
    [Display("Content", Description = "Translated broadcast HTML or XLIFF file")]
    public FileReference Content { get; set; } = new();

    [Display("Target language", Description = "Language code of the localized broadcast, such as de or de-DE")]
    public string Locale { get; set; } = string.Empty;

    [Display("Localized broadcast ID", Description = "Existing localized draft to update. A new draft is created when empty")]
    [DataSource(typeof(BroadcastDataHandler))]
    public string? ContentId { get; set; }
}
