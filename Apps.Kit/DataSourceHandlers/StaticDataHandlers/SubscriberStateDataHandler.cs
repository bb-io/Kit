using Apps.Kit.Constants;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Kit.DataSourceHandlers.StaticDataHandlers;

public class SubscriberStateDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData() =>
    [
        new(SubscriberStates.Active, "Active"),
        new(SubscriberStates.Bounced, "Bounced"),
        new(SubscriberStates.Cancelled, "Cancelled"),
        new(SubscriberStates.Complained, "Complained"),
        new(SubscriberStates.Inactive, "Inactive")
    ];
}
