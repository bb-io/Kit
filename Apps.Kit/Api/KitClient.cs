using System.Net;
using Apps.Kit.Constants;
using Apps.Kit.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Kit.Api;

public class KitClient() : BlackBirdRestClient(new()
{
    BaseUrl = Urls.Api.ToUri()
})
{
    private static readonly HttpStatusCode[] MisconfigurationStatusCodes =
    [
        HttpStatusCode.BadRequest,
        HttpStatusCode.Unauthorized,
        HttpStatusCode.Forbidden,
        HttpStatusCode.NotFound,
        HttpStatusCode.UnprocessableEntity
    ];

    public static string ExtractErrorMessage(RestResponse response)
    {
        if (string.IsNullOrWhiteSpace(response.Content))
            return response.ErrorMessage ?? "empty response body";

        try
        {
            var errors = JsonConvert.DeserializeObject<ErrorDto>(response.Content)?.Errors;
            if (errors is { Count: > 0 })
                return string.Join("; ", errors);
        }
        catch (JsonException)
        {
        }

        return response.Content;
    }

    public async Task<List<TItem>> Paginate<TPage, TItem>(RestRequest request, Func<List<TItem>, bool>? stopWhen = null)
        where TPage : PaginatedDto<TItem>
    {
        request.AddOrUpdateParameter(new QueryParameter("per_page", QueryParameters.MaxPageSize.ToString()));

        var items = new List<TItem>();
        string? cursor = null;

        do
        {
            if (cursor is not null)
                request.AddOrUpdateParameter(new QueryParameter("after", cursor));

            var page = await ExecuteWithErrorHandling<TPage>(request);
            items.AddRange(page.Items);

            if (stopWhen?.Invoke(page.Items) == true)
                break;

            cursor = page.Pagination?.HasNextPage == true ? page.Pagination.EndCursor : null;
        }
        while (!string.IsNullOrEmpty(cursor));

        return items;
    }

    protected override Exception ConfigureErrorException(RestResponse response)
    {
        var message = $"Kit API returned {(int)response.StatusCode} ({response.StatusCode}): {ExtractErrorMessage(response)}";

        return MisconfigurationStatusCodes.Contains(response.StatusCode)
            ? new PluginMisconfigurationException(message)
            : new PluginApplicationException(message);
    }
}
