using Apps.Kit.Models.Responses;
using Newtonsoft.Json;

namespace Apps.Kit.Models.Dtos;

public class SubscriberListDto : PaginatedDto<SubscriberResponse>
{
    [JsonProperty("subscribers")]
    public List<SubscriberResponse> Subscribers { get; set; } = [];

    public override List<SubscriberResponse> Items => Subscribers;
}
