using Apps.Kit.Models.Responses;
using Newtonsoft.Json;

namespace Apps.Kit.Models.Dtos;

public class SubscriberStatsEnvelopeDto
{
    [JsonProperty("subscriber")]
    public SubscriberStatsResponse? Subscriber { get; set; }
}
