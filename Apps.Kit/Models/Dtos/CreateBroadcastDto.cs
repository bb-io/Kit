using Newtonsoft.Json;

namespace Apps.Kit.Models.Dtos;

public class CreateBroadcastDto
{
    [JsonProperty("subject")]
    public string Subject { get; set; } = string.Empty;

    [JsonProperty("content")]
    public string Content { get; set; } = string.Empty;

    [JsonProperty("preview_text")]
    public string? PreviewText { get; set; }

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("public")]
    public bool Public { get; set; } = false;
}
