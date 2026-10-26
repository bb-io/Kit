using Newtonsoft.Json;
using Apps.Kit.Models.Dtos;

namespace Apps.Kit.Models.Responses;

public class SearchSubscribersResponse
{
    [JsonProperty("subscribers")]
    public List<SubscriberDto> Subscribers { get; set; } = [];

    [JsonProperty("pagination")]
    public PaginationDto? Pagination { get; set; }
}
