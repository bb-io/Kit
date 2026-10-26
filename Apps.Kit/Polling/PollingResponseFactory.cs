using Apps.Kit.Polling.Memory;
using Blackbird.Applications.Sdk.Common.Polling;

namespace Apps.Kit.Polling;

public static class PollingResponseFactory
{
    public static PollingEventResponse<PollingMemory, List<T>> Create<T>(List<T> items, DateTime pollingTime) => new()
    {
        FlyBird = items.Count > 0,
        Memory = new() { LastPollingTime = pollingTime },
        Result = items
    };
}
