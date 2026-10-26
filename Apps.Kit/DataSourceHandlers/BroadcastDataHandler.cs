
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Apps.Kit.Invocables;
using Apps.Kit.Api;
using Apps.Kit.Models.Responses;
using RestSharp;

namespace Apps.Kit.DataSourceHandlers;

public class BroadcastDataHandler(InvocationContext invocationContext)
    : KitInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(
        DataSourceContext context,
        CancellationToken cancellationToken)
    {
        const string endpoint = "broadcasts";

        var broadcasts = new List<DataSourceItem>();
        string? cursor = null;

        do
        {
            var request = new KitRequest(endpoint, Method.Get, Creds)
                .AddQueryParameter("per_page", 1000)
                .AddQueryParameter("slim", true);

            if (cursor is not null)
            {
                request.AddQueryParameter("after", cursor);
            }

            var response =
                await Client.ExecuteWithErrorHandling<SearchBroadcastsResponse>(request);

            broadcasts.AddRange(response.Broadcasts.Select(broadcast =>
                new DataSourceItem(
                    broadcast.Id,
                    $"{broadcast.Subject} ({broadcast.Id})")));

            cursor = response.Pagination?.HasNextPage == true
                ? response.Pagination.EndCursor
                : null;
        }
        while (cursor is not null);

        return broadcasts
            .Where(item => string.IsNullOrEmpty(context.SearchString) ||
                item.DisplayName.Contains(
                    context.SearchString,
                    StringComparison.OrdinalIgnoreCase));
    }
}
