using Newtonsoft.Json;

namespace Apps.Kit.Models.Dtos;

public class SubscriberTagDto
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("tagged_at")]
    public DateTime? TaggedAt { get; set; }
}
