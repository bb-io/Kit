using Newtonsoft.Json;
using Apps.Kit.Models.Dtos;

namespace Apps.Kit.Models.Responses;

public class GetSubscriberResponse
{
    [JsonProperty("subscriber")]
    public SubscriberDto? Subscriber { get; set; }
}
