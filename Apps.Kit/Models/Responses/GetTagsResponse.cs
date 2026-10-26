using Newtonsoft.Json;
using Apps.Kit.Models.Dtos;

namespace Apps.Kit.Models.Responses;

public class GetTagsResponse
{
    [JsonProperty("tags")]
    public List<TagDto> Tags { get; set; } = [];

    [JsonProperty("pagination")]
    public PaginationDto? Pagination { get; set; }
}
