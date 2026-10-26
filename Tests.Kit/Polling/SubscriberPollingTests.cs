using Apps.Kit.Polling;
using Apps.Kit.Polling.Memory;
using Blackbird.Applications.Sdk.Common.Polling;
using Newtonsoft.Json;
using Tests.Kit.Base;

namespace Tests.Kit.Polling;

[TestClass]
public class SubscriberPollingTests : TestBase
{
    [TestMethod]
    public async Task OnSubscriberCreated_WithoutMemory_ShouldNotFly()
    {
        var polling = new SubscriberPollingList(InvocationContext);

        var result = await polling.OnSubscriberCreated(new PollingEventRequest<PollingMemory>());

        Assert.IsFalse(result.FlyBird);
        Assert.IsNotNull(result.Memory);
    }

    [TestMethod]
    [TestCategory("Manual")]
    public async Task OnSubscriberCreated_WithMemory_ShouldReturnNewSubscribers()
    {
        var polling = new SubscriberPollingList(InvocationContext);
        var request = new PollingEventRequest<PollingMemory>
        {
            Memory = new() { LastPollingTime = DateTime.UtcNow.AddDays(-7) }
        };

        var result = await polling.OnSubscriberCreated(request);

        Assert.IsTrue(result.Result!.All(subscriber => subscriber.CreatedAt > request.Memory.LastPollingTime));
        Console.WriteLine(JsonConvert.SerializeObject(result.Result, Formatting.Indented));
    }

    [TestMethod]
    [TestCategory("Manual")]
    public async Task OnSubscriberUnsubscribed_WithMemory_ShouldReturnUnsubscribers()
    {
        var polling = new SubscriberPollingList(InvocationContext);
        var request = new PollingEventRequest<PollingMemory>
        {
            Memory = new() { LastPollingTime = DateTime.UtcNow.AddDays(-30) }
        };

        var result = await polling.OnSubscriberUnsubscribed(request);

        Assert.IsTrue(result.FlyBird);
        Assert.IsTrue(result.Result!.All(subscriber => subscriber.CanceledAt > request.Memory.LastPollingTime));
        Console.WriteLine(JsonConvert.SerializeObject(result.Result, Formatting.Indented));
    }
}
