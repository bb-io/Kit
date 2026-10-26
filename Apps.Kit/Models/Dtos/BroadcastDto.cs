using Newtonsoft.Json;

namespace Apps.Kit.Models.Dtos;

public class BroadcastDto
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("publication_id")]
    public string PublicationId { get; set; } = string.Empty;

    [JsonProperty("created_at")]
    public DateTime? CreatedAt { get; set; }

    [JsonProperty("subject")]
    public string Subject { get; set; } = string.Empty;

    [JsonProperty("preview_text")]
    public string? PreviewText { get; set; }

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("public")]
    public bool Public { get; set; }

    [JsonProperty("published_at")]
    public DateTime? PublishedAt { get; set; }

    [JsonProperty("send_at")]
    public DateTime? SendAt { get; set; }

    [JsonProperty("thumbnail_alt")]
    public string? ThumbnailAlt { get; set; }

    [JsonProperty("thumbnail_url")]
    public string? ThumbnailUrl { get; set; }

    [JsonProperty("public_url")]
    public string? PublicUrl { get; set; }

    [JsonProperty("email_address")]
    public string? EmailAddress { get; set; }

    [JsonProperty("email_template")]
    public EmailTemplateDto? EmailTemplate { get; set; }

    [JsonProperty("status")]
    public string? Status { get; set; }

    [JsonProperty("content")]
    public string? Content { get; set; }
}
