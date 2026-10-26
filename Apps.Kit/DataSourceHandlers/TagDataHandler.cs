using Apps.Kit.Api;
using Apps.Kit.Invocables;
using Apps.Kit.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Kit.DataSourceHandlers;

public class TagDataHandler(InvocationContext invocationContext)
    : KitInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var request = new KitRequest("tags", Method.Get, Creds);
        var tags = await Client.Paginate<TagListDto, TagDto>(request);

        return tags
            .Where(tag => string.IsNullOrEmpty(context.SearchString)
                || tag.Name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Select(tag => new DataSourceItem(tag.Id, tag.Name));
    }
}
