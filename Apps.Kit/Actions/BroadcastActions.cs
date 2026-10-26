using System.Net.Mime;
using Apps.Kit.Api;
using Apps.Kit.Constants;
using Apps.Kit.Extensions;
using Apps.Kit.Helpers;
using Apps.Kit.Invocables;
using Apps.Kit.Models.Dtos;
using Apps.Kit.Models.Html;
using Apps.Kit.Models.Requests;
using Apps.Kit.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Blackbird.Applications.SDK.Blueprints;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;

namespace Apps.Kit.Actions;

[ActionList("Broadcasts")]
public class BroadcastActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : KitInvocable(invocationContext)
{
    [Action("Search broadcasts", Description = "Search Kit broadcasts by status and send date")]
    public async Task<SearchBroadcastsResponse> SearchBroadcasts([ActionParameter] SearchBroadcastsRequest input)
    {
        var request = new KitRequest("broadcasts", Method.Get, Creds)
            .AddQueryParameter("slim", true)
            .AddOptionalQueryParameter("status", input.Status)
            .AddOptionalDateQueryParameter("sent_after", input.SentAfter)
            .AddOptionalDateQueryParameter("sent_before", input.SentBefore);

        var broadcasts = await Client.Paginate<BroadcastListDto, BroadcastResponse>(request);

        return new() { Broadcasts = broadcasts };
    }

    [Action("Get broadcast", Description = "Get details of a Kit broadcast")]
    public Task<BroadcastResponse> GetBroadcast([ActionParameter] BroadcastRequest input) =>
        GetBroadcastById(input.BroadcastId);

    [Action("Create broadcast", Description = "Create a new draft broadcast in Kit from an HTML file")]
    public async Task<BroadcastResponse> CreateBroadcast([ActionParameter] CreateBroadcastRequest input)
    {
        var payload = new BroadcastPayloadDto
        {
            Subject = input.Subject,
            Content = await fileManagementClient.DownloadStringAsync(input.Content),
            PreviewText = input.PreviewText,
            Description = input.Description
        };

        return await PostBroadcast(payload);
    }

    [BlueprintActionDefinition(BlueprintAction.DownloadContent)]
    [Action("Download broadcast", Description = "Download Kit broadcast content as HTML with Blackbird interoperability metadata")]
    public async Task<DownloadBroadcastResponse> DownloadBroadcast([ActionParameter] DownloadBroadcastRequest input)
    {
        var broadcast = await GetBroadcastById(input.ContentId);
        var html = BroadcastHtmlHelper.PrepareHtml(broadcast, input.SourceLanguage);
        var file = await fileManagementClient.UploadStringAsync(html, MediaTypeNames.Text.Html, $"broadcast-{broadcast.ContentId}.html");

        return new() { Content = file };
    }

    [BlueprintActionDefinition(BlueprintAction.UploadContent)]
    [Action("Upload broadcast", Description = "Upload translated HTML or XLIFF to Kit. Updates the specified localized draft or creates a new one")]
    public async Task<UploadBroadcastResponse> UploadBroadcast([ActionParameter] UploadBroadcastRequest input)
    {
        if (string.IsNullOrWhiteSpace(input.Locale))
            throw new PluginMisconfigurationException("Target language is required.");

        var content = await InteroperableFileHelper.ReadAsync(fileManagementClient, input.Content);
        var translated = BroadcastHtmlHelper.ProcessTranslatedHtml(content.Html);
        ValidateTranslatedHtml(translated);

        var broadcast = string.IsNullOrWhiteSpace(input.ContentId)
            ? await CreateLocalizedBroadcast(translated, input.Locale)
            : await UpdateLocalizedBroadcast(input.ContentId, translated, content);

        var file = await InteroperableFileHelper.UploadWithTargetReferenceAsync(
            fileManagementClient,
            content,
            broadcast.ContentId,
            broadcast.Subject,
            input.Locale,
            input.Content);

        return new() { Broadcast = broadcast, Content = file };
    }

    [Action("Delete broadcast", Description = "Delete a Kit broadcast")]
    public async Task DeleteBroadcast([ActionParameter] BroadcastRequest input)
    {
        var request = new KitRequest($"broadcasts/{input.BroadcastId}", Method.Delete, Creds);
        await Client.ExecuteWithErrorHandling(request);
    }

    [Action("Update broadcast", Description = "Update an existing Kit broadcast from an HTML file and keep it as a draft")]
    public async Task<BroadcastResponse> UpdateBroadcast([ActionParameter] UpdateBroadcastRequest input)
    {
        var payload = new BroadcastPayloadDto
        {
            Subject = input.Subject,
            Content = await fileManagementClient.DownloadStringAsync(input.Content),
            PreviewText = input.PreviewText,
            Description = input.Description
        };

        return await PutBroadcast(input.BroadcastId, payload);
    }

    private async Task<BroadcastResponse> CreateLocalizedBroadcast(ProcessedBroadcastHtml translated, string targetLanguage)
    {
        var source = await GetBroadcastById(translated.SourceBroadcastId!);
        var description = string.IsNullOrWhiteSpace(source.Description)
            ? $"[{targetLanguage}]"
            : $"{source.Description} [{targetLanguage}]";

        return await PostBroadcast(ToPayload(translated, description));
    }

    private async Task<BroadcastResponse> UpdateLocalizedBroadcast(
        string broadcastId,
        ProcessedBroadcastHtml translated,
        InteroperableHtmlContent content)
    {
        EnsureTargetIsNotSource(broadcastId, translated, content);

        var existing = await GetBroadcastById(broadcastId);
        if (!string.Equals(existing.Status, BroadcastStatuses.Draft, StringComparison.OrdinalIgnoreCase))
            throw new PluginMisconfigurationException("Only draft broadcasts can be updated.");

        return await PutBroadcast(broadcastId, ToPayload(translated, existing.Description));
    }

    private async Task<BroadcastResponse> GetBroadcastById(string broadcastId)
    {
        var request = new KitRequest($"broadcasts/{broadcastId}", Method.Get, Creds);
        return await ExecuteBroadcastRequest(request);
    }

    private async Task<BroadcastResponse> PostBroadcast(BroadcastPayloadDto payload)
    {
        var request = new KitRequest("broadcasts", Method.Post, Creds).WithJsonBody(payload);
        return await ExecuteBroadcastRequest(request);
    }

    private async Task<BroadcastResponse> PutBroadcast(string broadcastId, BroadcastPayloadDto payload)
    {
        var request = new KitRequest($"broadcasts/{broadcastId}", Method.Put, Creds).WithJsonBody(payload);
        return await ExecuteBroadcastRequest(request);
    }

    private async Task<BroadcastResponse> ExecuteBroadcastRequest(RestRequest request)
    {
        var response = await Client.ExecuteWithErrorHandling<BroadcastEnvelopeDto>(request);
        return response.Broadcast ?? throw new PluginApplicationException("Kit returned an empty broadcast response.");
    }

    private static BroadcastPayloadDto ToPayload(ProcessedBroadcastHtml translated, string? description) => new()
    {
        Subject = translated.Subject!,
        PreviewText = translated.PreviewText,
        Description = description,
        Content = translated.Html
    };

    private static void ValidateTranslatedHtml(ProcessedBroadcastHtml translated)
    {
        if (string.IsNullOrWhiteSpace(translated.SourceBroadcastId))
            throw new PluginMisconfigurationException("The translated file is missing the original broadcast ID.");

        if (string.IsNullOrWhiteSpace(translated.Subject))
            throw new PluginMisconfigurationException("The translated file is missing the broadcast subject.");
    }

    private static void EnsureTargetIsNotSource(
        string broadcastId,
        ProcessedBroadcastHtml translated,
        InteroperableHtmlContent content)
    {
        var sourceId = content.Transformation?.SourceSystemReference?.ContentId ?? translated.SourceBroadcastId;
        if (string.Equals(broadcastId, sourceId, StringComparison.Ordinal))
            throw new PluginMisconfigurationException("The localized broadcast ID cannot equal the source broadcast ID.");

        var targetId = content.Transformation?.TargetSystemReference?.ContentId;
        if (!string.IsNullOrWhiteSpace(targetId) && !string.Equals(broadcastId, targetId, StringComparison.Ordinal))
            throw new PluginMisconfigurationException("The XLIFF target broadcast ID does not match the localized broadcast ID.");
    }
}
