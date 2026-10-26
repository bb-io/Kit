using Newtonsoft.Json;

namespace Apps.Kit.Models.Dtos;

public class TagDto
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("created_at")]
    public DateTime? CreatedAt { get; set; }

    [JsonProperty("subscriber_count")]
    public int? SubscriberCount { get; set; }
}
