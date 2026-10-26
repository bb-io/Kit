using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Kit.DataSourceHandlers.EnumDataHandlers;

public class SubscriberStateDataHandler : IStaticDataSourceItemHandler
{
    private static Dictionary<string, string> EnumValues => new()
    {
        { "active", "Active" },
        { "bounced", "Bounced" },
        { "cancelled", "Cancelled" },
        { "complained", "Complained" },
        { "inactive", "Inactive" }
    };

    public IEnumerable<DataSourceItem> GetData()
    {
        return EnumValues.Select(x => new DataSourceItem(x.Key, x.Value));
    }
}
