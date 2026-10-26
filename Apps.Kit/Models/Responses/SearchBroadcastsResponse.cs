using Blackbird.Applications.Sdk.Common;

namespace Apps.Kit.Models.Responses;

public class SearchBroadcastsResponse
{
    [Display("Broadcasts")]
    public List<BroadcastResponse> Broadcasts { get; set; } = [];
}
