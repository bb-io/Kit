using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Kit.Models.Responses;

public class BroadcastResponse
{
    [Display("Broadcast ID")]
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [Display("Publication ID")]
    [JsonProperty("publication_id")]
    public string PublicationId { get; set; } = string.Empty;

    [Display("Created at")]
    [JsonProperty("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Display("Subject")]
    [JsonProperty("subject")]
    public string Subject { get; set; } = string.Empty;

    [Display("Preview text")]
    [JsonProperty("preview_text")]
    public string? PreviewText { get; set; }

    [Display("Description")]
    [JsonProperty("description")]
    public string? Description { get; set; }

    [Display("Public")]
    [JsonProperty("public")]
    public bool Public { get; set; }

    [Display("Published at")]
    [JsonProperty("published_at")]
    public DateTime? PublishedAt { get; set; }

    [Display("Send at")]
    [JsonProperty("send_at")]
    public DateTime? SendAt { get; set; }

    [Display("Thumbnail alt")]
    [JsonProperty("thumbnail_alt")]
    public string? ThumbnailAlt { get; set; }

    [Display("Thumbnail URL")]
    [JsonProperty("thumbnail_url")]
    public string? ThumbnailUrl { get; set; }

    [Display("Public URL")]
    [JsonProperty("public_url")]
    public string? PublicUrl { get; set; }

    [Display("Email address")]
    [JsonProperty("email_address")]
    public string? EmailAddress { get; set; }

    [Display("Email template")]
    [JsonProperty("email_template")]
    public EmailTemplateResponse? EmailTemplate { get; set; }

    [Display("Status")]
    [JsonProperty("status")]
    public string? Status { get; set; }

    [Display("Content")]
    [JsonProperty("content")]
    public string? Content { get; set; }
}
