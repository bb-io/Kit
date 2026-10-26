using Newtonsoft.Json;

namespace Apps.Kit.Models.Dtos;

public class SubscriberStatsDto
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("stats")]
    public StatsDto? Stats { get; set; }
}
