using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Apps.Kit.Invocables;
using Apps.Kit.Api;
using Apps.Kit.Models.Responses;
using RestSharp;

namespace Apps.Kit.DataSourceHandlers;

public class TagDataHandler(InvocationContext invocationContext)
    : KitInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(
        DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var endpoint = "tags";
        var request = new KitRequest(endpoint, Method.Get, Creds)
            .AddQueryParameter("per_page", 1000);

        var response =
            await Client.ExecuteWithErrorHandling<GetTagsResponse>(request);

        return response.Tags
            .Select(tag => new DataSourceItem(tag.Id, tag.Name));
    }
}
