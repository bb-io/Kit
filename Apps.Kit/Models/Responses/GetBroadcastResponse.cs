using Newtonsoft.Json;
using Apps.Kit.Models.Dtos;

namespace Apps.Kit.Models.Responses;

public class GetBroadcastResponse
{
    [JsonProperty("broadcast")]
    public BroadcastDto? Broadcast { get; set; }
}
