using Apps.Kit.Api;
using Apps.Kit.Constants;
using Apps.Kit.Extensions;
using Apps.Kit.Invocables;
using Apps.Kit.Models.Dtos;
using Apps.Kit.Models.Responses;
using Apps.Kit.Polling.Memory;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using RestSharp;

namespace Apps.Kit.Polling;

[PollingEventList("Broadcasts")]
public class BroadcastPollingList(InvocationContext invocationContext) : KitInvocable(invocationContext)
{
    private const int SentAfterBufferDays = 1;

    [PollingEvent("On broadcast published", Description = "Triggers when a Kit broadcast is published to the web")]
    [MultipleEvents]
    public async Task<PollingEventResponse<PollingMemory, List<BroadcastResponse>>> OnBroadcastPublished(
        PollingEventRequest<PollingMemory> request)
    {
        var pollingTime = DateTime.UtcNow;

        if (request.Memory is null)
            return PollingResponseFactory.Create(new List<BroadcastResponse>(), pollingTime);

        var broadcasts = await GetBroadcastsPublishedBetween(request.Memory.LastPollingTime, pollingTime);

        return PollingResponseFactory.Create(broadcasts, pollingTime);
    }

    private async Task<List<BroadcastResponse>> GetBroadcastsPublishedBetween(DateTime since, DateTime until)
    {
        var request = new KitRequest("broadcasts", Method.Get, Creds)
            .AddQueryParameter("slim", true)
            .AddQueryParameter("status", BroadcastStatuses.Completed)
            .AddOptionalDateQueryParameter("sent_after", since.AddDays(-SentAfterBufferDays));
        var broadcasts = await Client.Paginate<BroadcastListDto, BroadcastResponse>(request);

        return broadcasts
            .Where(broadcast => broadcast.Public && broadcast.PublishedAt > since && broadcast.PublishedAt <= until)
            .ToList();
    }
}
