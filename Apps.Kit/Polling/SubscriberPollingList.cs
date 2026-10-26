using Apps.Kit.Api;
using Apps.Kit.Invocables;
using Apps.Kit.Models.Dtos;
using Apps.Kit.Models.Responses;
using Apps.Kit.Polling.Memory;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using RestSharp;

namespace Apps.Kit.Polling;

[PollingEventList("Subscribers")]
public class SubscriberPollingList(
    InvocationContext invocationContext)
    : KitInvocable(invocationContext)
{
    [PollingEvent(
        "On subscriber created",
        Description = "Triggers when a new subscriber is created in Kit")]
    [MultipleEvents]
    public async Task<PollingEventResponse<
        SubscriberCreatedMemory,
        List<SubscriberDto>>> OnSubscriberCreated(
        PollingEventRequest<SubscriberCreatedMemory> request)
    {
        if (request.Memory is null)
        {
            return new()
            {
                FlyBird = false,
                Memory = new SubscriberCreatedMemory
                {
                    LastInteractionDate = DateTime.UtcNow
                },
                Result = []
            };
        }

        var memory = request.Memory;
        var currentCheckpoint = DateTime.UtcNow;
        var overlapStart = memory.LastInteractionDate.AddMinutes(-5);

        var subscribers = await GetCreatedSubscribers(overlapStart);

        var newSubscribers = FindNewSubscribers(
            subscribers,
            memory,
            currentCheckpoint);

        foreach (var subscriber in newSubscribers)
        {
            memory.KnownSubscriberIds[subscriber.Id] =
                subscriber.CreatedAt!.Value;
        }

        memory.LastInteractionDate = currentCheckpoint;

        CleanupMemory(memory);

        return new()
        {
            FlyBird = newSubscribers.Count > 0,
            Memory = memory,
            Result = newSubscribers
        };
    }


    [PollingEvent(
        "On subscriber unsubscribed",
        Description = "Triggers when a subscriber unsubscribes in Kit")]
    [MultipleEvents]
    public async Task<PollingEventResponse<
        SubscriberUnsubscribedMemory,
        List<SubscriberDto>>> OnSubscriberUnsubscribed(
        PollingEventRequest<SubscriberUnsubscribedMemory> request)
    {
        if (request.Memory is null)
        {
            return new()
            {
                FlyBird = false,
                Memory = new SubscriberUnsubscribedMemory
                {
                    LastInteractionDate = DateTime.UtcNow
                },
                Result = []
            };
        }

        var memory = request.Memory;
        var currentCheckpoint = DateTime.UtcNow;
        var overlapStart = memory.LastInteractionDate.AddMinutes(-5);

        var subscribers = await GetUnsubscribedSubscribers(overlapStart);

        var newUnsubscribers = FindNewUnsubscribers(
            subscribers,
            memory,
            currentCheckpoint);

        foreach (var subscriber in newUnsubscribers)
        {
            memory.KnownSubscriberIds[subscriber.Id] =
                subscriber.CanceledAt!.Value;
        }

        memory.LastInteractionDate = currentCheckpoint;

        var cutoff = memory.LastInteractionDate.AddMinutes(-5);

        var expiredIds = memory.KnownSubscriberIds
            .Where(entry => entry.Value <= cutoff)
            .Select(entry => entry.Key)
            .ToList();

        foreach (var id in expiredIds)
        {
            memory.KnownSubscriberIds.Remove(id);
        }

        return new()
        {
            FlyBird = newUnsubscribers.Count > 0,
            Memory = memory,
            Result = newUnsubscribers
        };
    }

    internal static List<SubscriberDto> FindNewUnsubscribers(
        List<SubscriberDto> subscribers,
        SubscriberUnsubscribedMemory memory,
        DateTime currentCheckpoint)
    {
        var overlapStart = memory.LastInteractionDate.AddMinutes(-5);

        return subscribers
            .Where(s => s.CanceledAt.HasValue
                     && s.CanceledAt.Value > overlapStart
                     && s.CanceledAt.Value <= currentCheckpoint
                     && !memory.KnownSubscriberIds.ContainsKey(s.Id))
            .GroupBy(s => s.Id)
            .Select(group => group.First())
            .ToList();
    }

    internal static List<SubscriberDto> FindNewSubscribers(
        List<SubscriberDto> subscribers,
        SubscriberCreatedMemory memory,
        DateTime currentCheckpoint)
    {
        var overlapStart = memory.LastInteractionDate.AddMinutes(-5);

        return subscribers
            .Where(s => s.CreatedAt.HasValue
                     && s.CreatedAt.Value > overlapStart
                     && s.CreatedAt.Value <= currentCheckpoint
                     && !memory.KnownSubscriberIds.ContainsKey(s.Id))
            .GroupBy(s => s.Id)
            .Select(group => group.First())
            .ToList();
    }

    internal static void CleanupMemory(
        SubscriberCreatedMemory memory)
    {
        var cutoff = memory.LastInteractionDate.AddMinutes(-5);

        var expiredIds = memory.KnownSubscriberIds
            .Where(entry => entry.Value <= cutoff)
            .Select(entry => entry.Key)
            .ToList();

        foreach (var id in expiredIds)
        {
            memory.KnownSubscriberIds.Remove(id);
        }
    }

    private async Task<List<SubscriberDto>> GetCreatedSubscribers(
        DateTime since)
    {
        var subscribers = new List<SubscriberDto>();
        string? cursor = null;

        do
        {
            var request = new KitRequest(
                "subscribers",
                Method.Get,
                Creds);

            request.AddQueryParameter("status", "all");
            request.AddQueryParameter("per_page", 1000);
            request.AddQueryParameter("sort_field", "created_at");
            request.AddQueryParameter("sort_order", "asc");

            // Request the previous calendar day to accommodate
            // Kit's documented date-based filtering.
            // Precise timestamp filtering happens in C#.
            request.AddQueryParameter(
                "created_after",
                since.AddDays(-1).ToString("yyyy-MM-dd"));

            if (cursor is not null)
            {
                request.AddQueryParameter("after", cursor);
            }

            var response =
                await Client.ExecuteWithErrorHandling<SearchSubscribersResponse>(
                    request);

            subscribers.AddRange(response.Subscribers);

            if (response.Pagination?.HasNextPage != true)
            {
                break;
            }

            cursor = response.Pagination.EndCursor;

            if (string.IsNullOrEmpty(cursor))
            {
                throw new InvalidOperationException(
                    "Kit reported another page without a cursor.");
            }
        }
        while (true);

        return subscribers;
    }


    private async Task<List<SubscriberDto>> GetUnsubscribedSubscribers(
        DateTime since)
    {
        var subscribers = new List<SubscriberDto>();
        string? cursor = null;

        do
        {
            var request = new KitRequest(
                "subscribers",
                Method.Get,
                Creds);

            request.AddQueryParameter("status", "cancelled");
            request.AddQueryParameter("include", "canceled_at");
            request.AddQueryParameter("per_page", 1000);
            request.AddQueryParameter("sort_field", "cancelled_at");
            request.AddQueryParameter("sort_order", "asc");

            // Retrieve a wider date range, then filter precisely
            // using CanceledAt in the polling logic.
            request.AddQueryParameter(
                "updated_after",
                since.AddDays(-1).ToString("yyyy-MM-dd"));

            if (cursor is not null)
            {
                request.AddQueryParameter("after", cursor);
            }

            var response =
                await Client.ExecuteWithErrorHandling<SearchSubscribersResponse>(
                    request);

            subscribers.AddRange(response.Subscribers);

            if (response.Pagination?.HasNextPage != true)
            {
                break;
            }

            cursor = response.Pagination.EndCursor;

            if (string.IsNullOrEmpty(cursor))
            {
                throw new InvalidOperationException(
                    "Kit reported another page without a cursor.");
            }
        }
        while (true);

        return subscribers;
    }
}
