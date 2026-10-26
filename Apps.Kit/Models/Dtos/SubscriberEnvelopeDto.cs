using Apps.Kit.Models.Responses;
using Newtonsoft.Json;

namespace Apps.Kit.Models.Dtos;

public class SubscriberEnvelopeDto
{
    [JsonProperty("subscriber")]
    public SubscriberResponse? Subscriber { get; set; }
}
