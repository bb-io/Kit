using Apps.Kit.Api;
using Apps.Kit.Invocables;
using Apps.Kit.Models.Dtos;
using Apps.Kit.Models.Responses;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Kit.DataSourceHandlers;

public class BroadcastDataHandler(InvocationContext invocationContext)
    : KitInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var request = new KitRequest("broadcasts", Method.Get, Creds).AddQueryParameter("slim", true);
        var broadcasts = await Client.Paginate<BroadcastListDto, BroadcastResponse>(request);

        return broadcasts
            .Select(broadcast => new DataSourceItem(broadcast.Id, $"{broadcast.Subject} ({broadcast.Id})"))
            .Where(item => string.IsNullOrEmpty(context.SearchString)
                || item.DisplayName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase));
    }
}
