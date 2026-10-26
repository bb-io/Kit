using Apps.Kit.Models.Responses;
using Newtonsoft.Json;

namespace Apps.Kit.Models.Dtos;

public class BroadcastListDto : PaginatedDto<BroadcastResponse>
{
    [JsonProperty("broadcasts")]
    public List<BroadcastResponse> Broadcasts { get; set; } = [];

    public override List<BroadcastResponse> Items => Broadcasts;
}
