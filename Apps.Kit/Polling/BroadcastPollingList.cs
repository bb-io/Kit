
using Apps.Kit.Invocables;
using Apps.Kit.Polling.Memory;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using Apps.Kit.Models.Dtos;
using Apps.Kit.Api;
using Apps.Kit.Models.Responses;
using RestSharp;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Kit.Polling;

[PollingEventList("Broadcasts")]
public class BroadcastPollingList(InvocationContext invocationContext)
    : KitInvocable(invocationContext)
{
    [PollingEvent(
        "On broadcast published",
        Description = "Triggers when a Kit broadcast is published to the web")]
    [MultipleEvents]
    public async Task<PollingEventResponse<
        BroadcastPublishedMemory,
        List<BroadcastDto>>> OnBroadcastPublished(
        PollingEventRequest<BroadcastPublishedMemory> request)
    {
        if (request.Memory is null)
        {
            return new()
            {
                FlyBird = false,
                Memory = new BroadcastPublishedMemory
                {
                    LastInteractionDate = DateTime.UtcNow
                },
                Result = []
            };
        }

        var memory = request.Memory;
        var currentCheckpoint = DateTime.UtcNow;
        var broadcasts = await GetPublishedBroadcasts();

        var newBroadcasts = FindNewBroadcasts(
            broadcasts,
            memory,
            currentCheckpoint);

        foreach (var broadcast in newBroadcasts)
        {
            memory.KnownBroadcastIds[broadcast.Id] = broadcast.PublishedAt!.Value;
        }

        memory.LastInteractionDate = currentCheckpoint;

        // NEW: Remove IDs outside our five-minute overlap window
        CleanupMemory(memory);

        return new()
        {
            FlyBird = newBroadcasts.Count > 0,
            Memory = memory,
            Result = newBroadcasts
        };
    }

    internal static List<BroadcastDto> FindNewBroadcasts(
        List<BroadcastDto> broadcasts,
        BroadcastPublishedMemory memory,
        DateTime currentCheckpoint)
    {
        var overlapStart = memory.LastInteractionDate.AddMinutes(-5);

        return broadcasts
            .Where(b => b.PublishedAt > overlapStart
                     && b.PublishedAt <= currentCheckpoint
                     && !memory.KnownBroadcastIds.ContainsKey(b.Id))
            .ToList();
    }

    // NEW: Clean up old broadcast IDs to prevent memory growth
    internal static void CleanupMemory(
        BroadcastPublishedMemory memory)
    {
        var cutoff = memory.LastInteractionDate.AddMinutes(-5);

        var expiredIds = memory.KnownBroadcastIds
            .Where(entry => entry.Value <= cutoff)
            .Select(entry => entry.Key)
            .ToList();

        foreach (var id in expiredIds)
        {
            memory.KnownBroadcastIds.Remove(id);
        }
    }

    private async Task<List<BroadcastDto>> GetPublishedBroadcasts()
    {
        const string endpoint = "broadcasts";

        var broadcasts = new List<BroadcastDto>();

        var request = new KitRequest(endpoint, Method.Get, Creds);

        request.AddQueryParameter("per_page", 1000);
        request.AddQueryParameter("slim", true);

        var response =
            await Client.ExecuteWithErrorHandling<SearchBroadcastsResponse>(request);

        broadcasts.AddRange(
            response.Broadcasts
                .Where(b => b.Public && b.PublishedAt.HasValue));

        while (response.Pagination?.HasNextPage == true &&
               response.Pagination.EndCursor is not null)
        {
            var nextRequest = new KitRequest(endpoint, Method.Get, Creds);

            nextRequest.AddQueryParameter("per_page", 1000);
            nextRequest.AddQueryParameter("slim", true);

            nextRequest.AddQueryParameter(
                "after",
                response.Pagination.EndCursor);

            response =
                await Client.ExecuteWithErrorHandling<SearchBroadcastsResponse>(nextRequest);

            broadcasts.AddRange(
                response.Broadcasts
                    .Where(b => b.Public && b.PublishedAt.HasValue));
        }

        return broadcasts;
    }
}
