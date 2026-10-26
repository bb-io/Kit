using Newtonsoft.Json;
using Apps.Kit.Models.Dtos;

namespace Apps.Kit.Models.Responses;

public class GetSubscriberStatsResponse
{
    [JsonProperty("subscriber")]
    public SubscriberStatsDto? Subscriber { get; set; }
}
