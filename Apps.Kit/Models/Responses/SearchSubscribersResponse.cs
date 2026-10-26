using Blackbird.Applications.Sdk.Common;

namespace Apps.Kit.Models.Responses;

public class SearchSubscribersResponse
{
    [Display("Subscribers")]
    public List<SubscriberResponse> Subscribers { get; set; } = [];
}
