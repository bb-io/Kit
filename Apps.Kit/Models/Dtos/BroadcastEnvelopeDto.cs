using Apps.Kit.Models.Responses;
using Newtonsoft.Json;

namespace Apps.Kit.Models.Dtos;

public class BroadcastEnvelopeDto
{
    [JsonProperty("broadcast")]
    public BroadcastResponse? Broadcast { get; set; }
}
