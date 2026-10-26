using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Apps.Kit.Invocables;
using Apps.Kit.Api;
using Apps.Kit.Models.Responses;
using RestSharp;

namespace Apps.Kit.DataSourceHandlers;

public class CustomFieldDataHandler(InvocationContext invocationContext)
    : KitInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(
        DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var endpoint = "custom_fields";
        var request = new KitRequest(endpoint, Method.Get, Creds)
            .AddQueryParameter("per_page", 1000);

        var response =
            await Client.ExecuteWithErrorHandling<GetCustomFieldsResponse>(request);

        return response.CustomFields
            .Select(field => new DataSourceItem(field.Key, field.Label));
    }
}
