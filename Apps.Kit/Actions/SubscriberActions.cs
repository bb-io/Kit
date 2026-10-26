using Apps.Kit.Api;
using Apps.Kit.Extensions;
using Apps.Kit.Invocables;
using Apps.Kit.Models.Dtos;
using Apps.Kit.Models.Requests;
using Apps.Kit.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using RestSharp;

namespace Apps.Kit.Actions;

[ActionList("Subscribers")]
public class SubscriberActions(InvocationContext invocationContext) : KitInvocable(invocationContext)
{
    [Action("Search subscribers", Description = "Search Kit subscribers by status, email address and dates")]
    public async Task<SearchSubscribersResponse> SearchSubscribers([ActionParameter] SearchSubscribersRequest input)
    {
        var request = new KitRequest("subscribers", Method.Get, Creds)
            .AddOptionalQueryParameter("status", input.Status)
            .AddOptionalQueryParameter("email_address", input.EmailAddress)
            .AddOptionalDateQueryParameter("created_after", input.CreatedAfter)
            .AddOptionalDateQueryParameter("created_before", input.CreatedBefore)
            .AddOptionalDateQueryParameter("updated_after", input.UpdatedAfter)
            .AddOptionalDateQueryParameter("updated_before", input.UpdatedBefore);

        var subscribers = await Client.Paginate<SubscriberListDto, SubscriberResponse>(request);

        return new() { Subscribers = subscribers };
    }

    [Action("Get subscriber", Description = "Get details of a Kit subscriber")]
    public Task<SubscriberResponse> GetSubscriber([ActionParameter] SubscriberRequest input) =>
        GetSubscriberById(input.SubscriberId);

    [Action("Get subscriber tags", Description = "Get all tags of a Kit subscriber")]
    public async Task<GetSubscriberTagsResponse> GetSubscriberTags([ActionParameter] SubscriberRequest input)
    {
        var request = new KitRequest($"subscribers/{input.SubscriberId}/tags", Method.Get, Creds);
        var tags = await Client.Paginate<SubscriberTagListDto, SubscriberTagResponse>(request);

        return new() { Tags = tags };
    }

    [Action("Get subscriber stats", Description = "Get email engagement statistics of a Kit subscriber")]
    public async Task<SubscriberStatsResponse> GetSubscriberStats([ActionParameter] GetSubscriberStatsRequest input)
    {
        var request = new KitRequest($"subscribers/{input.SubscriberId}/stats", Method.Get, Creds)
            .AddOptionalDateQueryParameter("email_sent_after", input.SentAfter)
            .AddOptionalDateQueryParameter("email_sent_before", input.SentBefore);

        var response = await Client.ExecuteWithErrorHandling<SubscriberStatsEnvelopeDto>(request);

        return response.Subscriber ?? throw new PluginApplicationException("Kit returned an empty subscriber stats response.");
    }

    [Action("Create subscriber", Description = "Create a new subscriber or update an existing subscriber with the same email address")]
    public Task<SubscriberResponse> CreateSubscriber([ActionParameter] CreateSubscriberRequest input)
    {
        var payload = new CreateSubscriberDto
        {
            EmailAddress = input.EmailAddress,
            FirstName = input.FirstName,
            State = input.State
        };

        var request = new KitRequest("subscribers", Method.Post, Creds).WithJsonBody(payload);

        return ExecuteSubscriberRequest(request);
    }

    [Action("Add tag to subscriber", Description = "Apply a Kit tag to a subscriber")]
    public Task<SubscriberResponse> AddTagToSubscriber([ActionParameter] TagSubscriberRequest input)
    {
        var request = new KitRequest($"tags/{input.TagId}/subscribers/{input.SubscriberId}", Method.Post, Creds);
        return ExecuteSubscriberRequest(request);
    }

    [Action("Delete tag from subscriber", Description = "Remove a Kit tag from a subscriber")]
    public async Task DeleteTagFromSubscriber([ActionParameter] TagSubscriberRequest input)
    {
        var request = new KitRequest($"tags/{input.TagId}/subscribers/{input.SubscriberId}", Method.Delete, Creds);
        await Client.ExecuteWithErrorHandling(request);
    }

    [Action("Update subscriber", Description = "Update the email address or first name of a Kit subscriber")]
    public async Task<SubscriberResponse> UpdateSubscriber([ActionParameter] UpdateSubscriberRequest input)
    {
        var payload = new UpdateSubscriberDto
        {
            EmailAddress = string.IsNullOrEmpty(input.EmailAddress)
                ? (await GetSubscriberById(input.SubscriberId)).EmailAddress
                : input.EmailAddress,
            FirstName = input.FirstName
        };

        return await PutSubscriber(input.SubscriberId, payload);
    }

    [Action("Update subscriber custom field", Description = "Update a custom field value of a Kit subscriber")]
    public async Task<SubscriberResponse> UpdateSubscriberCustomField([ActionParameter] UpdateSubscriberCustomFieldRequest input)
    {
        var subscriber = await GetSubscriberById(input.SubscriberId);
        var payload = new UpdateSubscriberDto
        {
            EmailAddress = subscriber.EmailAddress,
            Fields = new() { [input.CustomField] = input.Value }
        };

        return await PutSubscriber(input.SubscriberId, payload);
    }

    [Action("Unsubscribe subscriber", Description = "Unsubscribe a Kit subscriber from all emails")]
    public async Task UnsubscribeSubscriber([ActionParameter] SubscriberRequest input)
    {
        var request = new KitRequest($"subscribers/{input.SubscriberId}/unsubscribe", Method.Post, Creds);
        await Client.ExecuteWithErrorHandling(request);
    }

    private Task<SubscriberResponse> GetSubscriberById(string subscriberId)
    {
        var request = new KitRequest($"subscribers/{subscriberId}", Method.Get, Creds);
        return ExecuteSubscriberRequest(request);
    }

    private Task<SubscriberResponse> PutSubscriber(string subscriberId, UpdateSubscriberDto payload)
    {
        var request = new KitRequest($"subscribers/{subscriberId}", Method.Put, Creds).WithJsonBody(payload);
        return ExecuteSubscriberRequest(request);
    }

    private async Task<SubscriberResponse> ExecuteSubscriberRequest(RestRequest request)
    {
        var response = await Client.ExecuteWithErrorHandling<SubscriberEnvelopeDto>(request);
        return response.Subscriber ?? throw new PluginApplicationException("Kit returned an empty subscriber response.");
    }
}
