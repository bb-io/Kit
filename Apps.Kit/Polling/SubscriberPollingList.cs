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

[PollingEventList("Subscribers")]
public class SubscriberPollingList(InvocationContext invocationContext) : KitInvocable(invocationContext)
{
    [PollingEvent("On subscriber created", Description = "Triggers when a new subscriber is created in Kit")]
    [MultipleEvents]
    public async Task<PollingEventResponse<PollingMemory, List<SubscriberResponse>>> OnSubscriberCreated(
        PollingEventRequest<PollingMemory> request)
    {
        var pollingTime = DateTime.UtcNow;

        if (request.Memory is null)
            return PollingResponseFactory.Create(new List<SubscriberResponse>(), pollingTime);

        var subscribers = await GetSubscribersCreatedBetween(request.Memory.LastPollingTime, pollingTime);

        return PollingResponseFactory.Create(subscribers, pollingTime);
    }

    [PollingEvent("On subscriber unsubscribed", Description = "Triggers when a subscriber unsubscribes in Kit")]
    [MultipleEvents]
    public async Task<PollingEventResponse<PollingMemory, List<SubscriberResponse>>> OnSubscriberUnsubscribed(
        PollingEventRequest<PollingMemory> request)
    {
        var pollingTime = DateTime.UtcNow;

        if (request.Memory is null)
            return PollingResponseFactory.Create(new List<SubscriberResponse>(), pollingTime);

        var subscribers = await GetSubscribersUnsubscribedBetween(request.Memory.LastPollingTime, pollingTime);

        return PollingResponseFactory.Create(subscribers, pollingTime);
    }

    private async Task<List<SubscriberResponse>> GetSubscribersCreatedBetween(DateTime since, DateTime until)
    {
        var request = new KitRequest("subscribers", Method.Get, Creds)
            .AddQueryParameter("status", SubscriberStates.All)
            .AddDateTimeQueryParameter("created_after", since);

        var subscribers = await Client.Paginate<SubscriberListDto, SubscriberResponse>(request);

        return subscribers
            .Where(subscriber => subscriber.CreatedAt > since && subscriber.CreatedAt <= until)
            .ToList();
    }

    private async Task<List<SubscriberResponse>> GetSubscribersUnsubscribedBetween(DateTime since, DateTime until)
    {
        var request = new KitRequest("subscribers", Method.Get, Creds)
            .AddQueryParameter("status", SubscriberStates.Cancelled)
            .AddQueryParameter("include", "canceled_at")
            .AddQueryParameter("sort_field", "cancelled_at")
            .AddQueryParameter("sort_order", "desc");

        var subscribers = await Client.Paginate<SubscriberListDto, SubscriberResponse>(
            request,
            page => page.Any(subscriber => subscriber.CanceledAt <= since));

        return subscribers
            .Where(subscriber => subscriber.CanceledAt > since && subscriber.CanceledAt <= until)
            .ToList();
    }
}
