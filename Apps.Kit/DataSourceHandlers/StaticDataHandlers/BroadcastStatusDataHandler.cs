using Apps.Kit.Constants;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Kit.DataSourceHandlers.StaticDataHandlers;

public class BroadcastStatusDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData() =>
    [
        new(BroadcastStatuses.Draft, "Draft"),
        new(BroadcastStatuses.Scheduled, "Scheduled"),
        new(BroadcastStatuses.Sending, "Sending"),
        new(BroadcastStatuses.Completed, "Completed"),
        new(BroadcastStatuses.Aborted, "Aborted")
    ];
}
