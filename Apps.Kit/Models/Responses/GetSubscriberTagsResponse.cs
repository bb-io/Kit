using Blackbird.Applications.Sdk.Common;

namespace Apps.Kit.Models.Responses;

public class GetSubscriberTagsResponse
{
    [Display("Tags")]
    public List<SubscriberTagResponse> Tags { get; set; } = [];
}
