using Apps.Kit.Api;
using Apps.Kit.Invocables;
using Apps.Kit.Models.Requests;
using Apps.Kit.Models.Responses;
using Apps.Kit.Models.Dtos;
using Apps.Kit.Helpers;
using Blackbird.Filters.Transformations;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;
using System.Text;

namespace Apps.Kit.Actions;

[ActionList("Broadcasts")]
public class BroadcastActions(
    InvocationContext invocationContext,
    IFileManagementClient fileManagementClient)
    : KitInvocable(invocationContext)
{
    [Action("Get a broadcast",
           Description = "Get details about a broadcast")]
    public async Task<BroadcastDto> GetBroadcast([ActionParameter] BroadcastRequest input)
    {
        var endpoint = $"broadcasts/{input.BroadcastId}";
        var request = new KitRequest(endpoint, Method.Get, Creds);

        var response = await Client.ExecuteWithErrorHandling<GetBroadcastResponse>(request);

        if (response.Broadcast is null)
        {
            throw new PluginApplicationException("Kit returned an empty broadcast response.");
        }

        return response.Broadcast;
    }

    [Action("Create a broadcast",
    Description = "Create a new draft broadcast in Kit")]
    public async Task<BroadcastDto> CreateBroadcast(
    [ActionParameter] CreateBroadcastRequest input)
    {
        var html = await ReadHtmlFile(input.Content);

        var dto = new CreateBroadcastDto
        {
            Subject = input.Subject,
            Content = html,
            PreviewText = input.PreviewText,
            Description = input.Description
        };

        var request = new KitRequest("broadcasts", Method.Post, Creds);
        request.AddJsonBody(dto);

        var response =
            await Client.ExecuteWithErrorHandling<GetBroadcastResponse>(request);

        if (response.Broadcast is null)
        {
            throw new PluginApplicationException(
                "Kit returned an empty broadcast response.");
        }

        return response.Broadcast;
    }


    [Action("Create localized broadcast",
        Description = "Create a new Kit draft from translated HTML or XLIFF")]
    public async Task<CreateLocalizedBroadcastResponse> CreateLocalizedBroadcast(
        [ActionParameter] CreateLocalizedBroadcastRequest input)
    {
        if (string.IsNullOrWhiteSpace(input.TargetLanguage))
        {
            throw new PluginApplicationException(
                "Target language is required.");
        }

        // Read and process the translated HTML.
        var interoperableContent = await ReadInteroperableHtmlFile(input.Content);

        var processed = BroadcastHtmlHelper.ProcessTranslatedHtml(
            interoperableContent.Html);

        // Ensure the translated file identifies its source.
        if (string.IsNullOrWhiteSpace(processed.SourceBroadcastId))
        {
            throw new PluginApplicationException(
                "The translated file is missing the original broadcast ID.");
        }

        if (string.IsNullOrWhiteSpace(processed.Subject))
        {
            throw new PluginApplicationException(
                "The translated file is missing the broadcast subject.");
        }

        // Retrieve the original broadcast to preserve its description.
        var original = await GetBroadcast(new BroadcastRequest
        {
            BroadcastId = processed.SourceBroadcastId
        });

        // Identify the localized version in Kit.
        var description = string.IsNullOrWhiteSpace(original.Description)
            ? $"[{input.TargetLanguage}]"
            : $"{original.Description} [{input.TargetLanguage}]";

        // Create a new draft using Kit's existing API.
        var dto = new CreateBroadcastDto
        {
            Subject = processed.Subject,
            PreviewText = processed.PreviewText,
            Description = description,
            Content = processed.Html
        };

        var request = new KitRequest("broadcasts", Method.Post, Creds);
        request.AddJsonBody(dto);

        var response =
            await Client.ExecuteWithErrorHandling<GetBroadcastResponse>(request);

        if (response.Broadcast is null)
        {
            throw new PluginApplicationException(
                "Kit returned an empty broadcast response.");
        }


        var createdBroadcast = response.Broadcast;

        FileReference? outputContent = null;

        var transformationResult = interoperableContent.TransformationResult;

        if (transformationResult?.Success == true)
        {
            var transformation = transformationResult.Value;

            // Associate the translated content with the newly created Kit draft.
            transformation.TargetSystemReference.ContentId = createdBroadcast.Id;
            transformation.TargetSystemReference.ContentName = createdBroadcast.Subject;
            transformation.TargetSystemReference.SystemName = "Kit";
            transformation.TargetSystemReference.SystemRef = "https://kit.com/";
            transformation.TargetLanguage = input.TargetLanguage;

            if (transformationResult.WasBilingual)
            {
                // Preserve the bilingual XLIFF and its updated metadata.
                using var stream = transformation.ToStream();

                outputContent = await fileManagementClient.UploadAsync(
                    stream,
                    "application/xliff+xml",
                    transformation.BilingualFileName);
            }
            else
            {
                // Return the reconstructed target file with its system reference.
                var targetResult = transformation.Target();

                if (!targetResult.Success)
                {
                    throw new PluginApplicationException(
                        $"Could not reconstruct localized content: {targetResult.Error}");
                }

                var target = targetResult.Value;
                target.SystemReference = transformation.TargetSystemReference;

                using var stream = target.ToStream();

                outputContent = await fileManagementClient.UploadAsync(
                    stream,
                    target.OriginalMediaType,
                    target.OriginalName);
            }
        }
        else
        {
            // A regular HTML file has no transformation to preserve.
            // Return the original input file for now.
            outputContent = input.Content;
        }

        return new CreateLocalizedBroadcastResponse
        {
            Broadcast = createdBroadcast,
            Content = outputContent,
            TargetLanguage = input.TargetLanguage
        };
    }

    [Action("Update localized broadcast",
        Description = "Update an existing Kit draft using translated HTML or XLIFF")]
    public async Task<CreateLocalizedBroadcastResponse> UpdateLocalizedBroadcast(
        [ActionParameter] UpdateLocalizedBroadcastRequest input)
    {
        if (string.IsNullOrWhiteSpace(input.BroadcastId))
            throw new PluginApplicationException(
                "Localized broadcast ID is required.");

        if (string.IsNullOrWhiteSpace(input.TargetLanguage))
            throw new PluginApplicationException(
                "Target language is required.");

        // Extract translated HTML while retaining XLIFF metadata.
        var interoperableContent = await ReadInteroperableHtmlFile(input.Content);

        var processed = BroadcastHtmlHelper.ProcessTranslatedHtml(
            interoperableContent.Html);

        if (string.IsNullOrWhiteSpace(processed.SourceBroadcastId))
            throw new PluginApplicationException(
                "The translated file is missing the original broadcast ID.");


        // Validate source and target identities before updating Kit.
        var transformationResult = interoperableContent.TransformationResult;

        if (transformationResult?.Success == true)
        {
            var sourceId =
                transformationResult.Value.SourceSystemReference?.ContentId;

            var targetId =
                transformationResult.Value.TargetSystemReference?.ContentId;

            if (string.IsNullOrWhiteSpace(sourceId))
                throw new PluginApplicationException(
                    "The XLIFF is missing the original source broadcast ID.");

            // Never allow the original source broadcast to be overwritten.
            if (string.Equals(
                input.BroadcastId,
                sourceId,
                StringComparison.Ordinal))
            {
                throw new PluginApplicationException(
                    "The localized broadcast ID cannot equal the source broadcast ID.");
            }

            // An existing XLIFF target must match the requested draft.
            if (!string.IsNullOrWhiteSpace(targetId) &&
                !string.Equals(
                    input.BroadcastId,
                    targetId,
                    StringComparison.Ordinal))
            {
                throw new PluginApplicationException(
                    "The XLIFF target broadcast ID does not match the requested draft.");
            }
        }
        else
        {
            // For ordinary HTML, blackbird-ucid identifies the source.
            if (string.Equals(
                input.BroadcastId,
                processed.SourceBroadcastId,
                StringComparison.Ordinal))
            {
                throw new PluginApplicationException(
                    "The localized broadcast ID cannot equal the source broadcast ID.");
            }
        }

        // Verify the target exists and is editable.
        var existing = await GetBroadcast(new BroadcastRequest
        {
            BroadcastId = input.BroadcastId
        });

        if (!string.Equals(
            existing.Status,
            "draft",
            StringComparison.OrdinalIgnoreCase))
        {
            throw new PluginApplicationException(
                "Only draft broadcasts can be updated.");
        }

        if (string.IsNullOrWhiteSpace(processed.Subject))
            throw new PluginApplicationException(
                "The translated file is missing the broadcast subject.");

        // Preserve the existing description.
        var dto = new UpdateBroadcastDto
        {
            Subject = processed.Subject,
            PreviewText = processed.PreviewText,
            Description = existing.Description,
            Content = processed.Html
        };

        var request = new KitRequest(
            $"broadcasts/{input.BroadcastId}",
            Method.Put,
            Creds);

        request.AddJsonBody(dto);

        var response = await Client.ExecuteWithErrorHandling<GetBroadcastResponse>(
            request);

        if (response.Broadcast is null)
            throw new PluginApplicationException(
                "Kit returned an empty broadcast response.");

        var updatedBroadcast = response.Broadcast;

        // Return the translated file with updated Blacklake metadata.
        FileReference? outputContent = null;

        if (transformationResult?.Success == true)
        {
            var outputTransformation = transformationResult.Value;

            if (outputTransformation.TargetSystemReference is null)
            {
                outputTransformation.TargetSystemReference =
                    new Blackbird.Filters.Shared.SystemReference();
            }

            outputTransformation.TargetSystemReference.ContentId =
                updatedBroadcast.Id;

            outputTransformation.TargetSystemReference.ContentName =
                updatedBroadcast.Subject;

            outputTransformation.TargetSystemReference.SystemName = "Kit";
            outputTransformation.TargetSystemReference.SystemRef =
                "https://kit.com/";

            outputTransformation.TargetLanguage = input.TargetLanguage;

            if (transformationResult.WasBilingual)
            {
                using var stream = outputTransformation.ToStream();

                outputContent = await fileManagementClient.UploadAsync(
                    stream,
                    "application/xliff+xml",
                    outputTransformation.BilingualFileName);
            }
            else
            {
                var targetResult = outputTransformation.Target();

                if (!targetResult.Success)
                    throw new PluginApplicationException(
                        $"Could not reconstruct localized content: {targetResult.Error}");

                var target = targetResult.Value;

                target.SystemReference =
                    outputTransformation.TargetSystemReference;

                using var stream = target.ToStream();

                outputContent = await fileManagementClient.UploadAsync(
                    stream,
                    target.OriginalMediaType,
                    target.OriginalName);
            }
        }
        else
        {
            outputContent = input.Content;
        }

        return new CreateLocalizedBroadcastResponse
        {
            Broadcast = updatedBroadcast,
            Content = outputContent,
            TargetLanguage = input.TargetLanguage
        };
    }

    [Action("Update a broadcast",
    Description = "Update an existing Kit broadcast as a draft")]
    public async Task<BroadcastDto> UpdateBroadcast(
    [ActionParameter] UpdateBroadcastRequest input)
    {
        var html = await ReadHtmlFile(input.Content);

        var dto = new UpdateBroadcastDto
        {
            Subject = input.Subject,
            Content = html,
            PreviewText = input.PreviewText,
            Description = input.Description
        };

        var endpoint = $"broadcasts/{input.BroadcastId}";
        var request = new KitRequest(endpoint, Method.Put, Creds);
        request.AddJsonBody(dto);

        var response =
            await Client.ExecuteWithErrorHandling<GetBroadcastResponse>(request);

        if (response.Broadcast is null)
        {
            throw new PluginApplicationException(
                "Kit returned an empty broadcast response.");
        }

        return response.Broadcast;
    }

    [Action("Delete a broadcast",
            Description = "Delete a specified Kit broadcast")]
    public async Task DeleteBroadcast([ActionParameter] BroadcastRequest input)
    {
        var endpoint = $"broadcasts/{input.BroadcastId}";
        var request = new KitRequest(endpoint, Method.Delete, Creds);

        await Client.ExecuteWithErrorHandling(request);
    }

    [Action("Download broadcast",
        Description = "Download Kit broadcast content with Blackbird interoperability metadata")]
    public async Task<DownloadBroadcastResponse> DownloadBroadcast(
        [ActionParameter] DownloadBroadcastRequest input)
    {
        var broadcast = await GetBroadcast(new BroadcastRequest
        {
            BroadcastId = input.BroadcastId
        });

        if (string.IsNullOrWhiteSpace(broadcast.Content))
        {
            throw new PluginApplicationException(
                "This broadcast has no HTML content.");
        }

        var html = BroadcastHtmlHelper.PrepareHtml(
            broadcast,
            input.SourceLanguage);

        var bytes = Encoding.UTF8.GetBytes(html);
        using var stream = new MemoryStream(bytes);

        var file = await fileManagementClient.UploadAsync(
            stream,
            "text/html",
            $"broadcast-{broadcast.Id}.html");

        return new DownloadBroadcastResponse
        {
            Content = file
        };
    }

    [Action("Search broadcasts",
        Description = "Search available Kit broadcasts")]
    public async Task<List<BroadcastDto>> SearchBroadcasts(
        [ActionParameter] SearchBroadcastsRequest input)
    {
        var endpoint = "broadcasts";
        var request = new KitRequest(endpoint, Method.Get, Creds);

        AddSearchParameters(request, input);

        var response =
            await Client.ExecuteWithErrorHandling<SearchBroadcastsResponse>(request);

        var broadcasts = response.Broadcasts.ToList();

        while (response.Pagination?.HasNextPage == true &&
               response.Pagination.EndCursor is not null)
        {
            var nextRequest = new KitRequest(endpoint, Method.Get, Creds);

            AddSearchParameters(nextRequest, input);

            nextRequest.AddQueryParameter(
                "after",
                response.Pagination.EndCursor);

            response =
                await Client.ExecuteWithErrorHandling<SearchBroadcastsResponse>(nextRequest);

            broadcasts.AddRange(response.Broadcasts);
        }

        return broadcasts;
    }

    private void AddSearchParameters(
        KitRequest request,
        SearchBroadcastsRequest input)
    {
        request.AddQueryParameter("per_page", 1000);
        request.AddQueryParameter("slim", true);

        if (!string.IsNullOrEmpty(input.Status))
        {
            request.AddQueryParameter("status", input.Status);
        }

        if (input.SentAfter.HasValue)
        {
            request.AddQueryParameter(
                "sent_after",
                input.SentAfter.Value.ToString("yyyy-MM-dd"));
        }

        if (input.SentBefore.HasValue)
        {
            request.AddQueryParameter(
                "sent_before",
                input.SentBefore.Value.ToString("yyyy-MM-dd"));
        }
    }

    private async Task<string> ReadHtmlFile(FileReference content)
    {
        var file = await fileManagementClient.DownloadAsync(content);
        var bytes = await file.GetByteData();

        return Encoding.UTF8.GetString(bytes);
    }

    private async Task<InteroperableHtmlContent> ReadInteroperableHtmlFile(
        FileReference content)
    {
        var file = await fileManagementClient.DownloadAsync(content);

        var transformationResult = Transformation.Load(
            file,
            content.Name,
            content.ContentType);

        if (transformationResult.Success)
        {
            var targetResult = transformationResult.Target();

            if (!targetResult.Success)
            {
                throw new PluginApplicationException(
                    $"Could not extract translated content: {targetResult.Error}");
            }

            using var targetStream = targetResult.Value.ToStream();
            using var targetReader = new StreamReader(targetStream);

            return new InteroperableHtmlContent
            {
                Html = await targetReader.ReadToEndAsync(),
                TransformationResult = transformationResult
            };
        }

        // Regular HTML files are still supported.
        InvocationContext.Logger?.LogInformation(
            $"Not a Blackbird interoperable file: {transformationResult.Error}",
            []);

        using var reader = new StreamReader(file);

        return new InteroperableHtmlContent
        {
            Html = await reader.ReadToEndAsync(),
            TransformationResult = null
        };
    }
}
