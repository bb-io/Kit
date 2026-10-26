using Newtonsoft.Json;
using Apps.Kit.Models.Dtos;

namespace Apps.Kit.Models.Responses;

public class GetSubscriberTagsResponse
{
    [JsonProperty("tags")]
    public List<SubscriberTagDto> Tags { get; set; } = [];

    [JsonProperty("pagination")]
    public PaginationDto? Pagination { get; set; }
}
