using Newtonsoft.Json;
using Apps.Kit.Models.Dtos;

namespace Apps.Kit.Models.Responses;

public class SearchBroadcastsResponse
{
    [JsonProperty("broadcasts")]
    public List<BroadcastDto> Broadcasts { get; set; } = [];

    [JsonProperty("pagination")]
    public PaginationDto? Pagination { get; set; }
}
