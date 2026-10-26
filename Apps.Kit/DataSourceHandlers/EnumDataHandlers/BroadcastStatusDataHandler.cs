using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Kit.DataSourceHandlers.EnumDataHandlers;

public class BroadcastStatusDataHandler : IStaticDataSourceItemHandler
{
    private static Dictionary<string, string> EnumValues => new()
    {
        { "draft", "Draft" },
        { "scheduled", "Scheduled" },
        { "sending", "Sending" },
        { "completed", "Completed" },
        { "aborted", "Aborted" }
    };

    public IEnumerable<DataSourceItem> GetData()
    {
        return EnumValues.Select(x => new DataSourceItem(x.Key, x.Value));
    }
}
