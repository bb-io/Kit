using Apps.Kit.Models.Responses;
using Newtonsoft.Json;

namespace Apps.Kit.Models.Dtos;

public class SubscriberTagListDto : PaginatedDto<SubscriberTagResponse>
{
    [JsonProperty("tags")]
    public List<SubscriberTagResponse> Tags { get; set; } = [];

    public override List<SubscriberTagResponse> Items => Tags;
}
