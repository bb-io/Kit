using Apps.Kit.Api;
using Apps.Kit.Invocables;
using Apps.Kit.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Kit.DataSourceHandlers;

public class CustomFieldDataHandler(InvocationContext invocationContext)
    : KitInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var request = new KitRequest("custom_fields", Method.Get, Creds);
        var customFields = await Client.Paginate<CustomFieldListDto, CustomFieldDto>(request);

        return customFields
            .Where(field => string.IsNullOrEmpty(context.SearchString)
                || field.Label.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Select(field => new DataSourceItem(field.Key, field.Label));
    }
}
