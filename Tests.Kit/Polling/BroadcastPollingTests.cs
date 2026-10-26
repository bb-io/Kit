using Apps.Kit.Models.Dtos;
using Apps.Kit.Polling;
using Apps.Kit.Polling.Memory;
using Blackbird.Applications.Sdk.Common.Polling;
using Tests.Kit.Base;
using Newtonsoft.Json;

namespace Tests.Kit.Polling;

[TestClass]
public class BroadcastPollingTests : TestBase
{
    [TestMethod]
    public void FindNewBroadcasts_ReturnsOnlyNewBroadcasts()
    {
        var now = DateTime.UtcNow;

        var memory = new BroadcastPublishedMemory
        {
            LastInteractionDate = now.AddMinutes(-10),
            KnownBroadcastIds = new Dictionary<string, DateTime>
{
    { "3", now.AddMinutes(-1) }
}
        };

        var broadcasts = new List<BroadcastDto>
        {
            new()
            {
                Id = "1",
                PublishedAt = now.AddMinutes(-2)
            },
            new()
            {
                Id = "2",
                PublishedAt = now.AddMinutes(-20)
            },
            new()
            {
                Id = "3",
                PublishedAt = now.AddMinutes(-1)
            }
        };

        var result = BroadcastPollingList.FindNewBroadcasts(
            broadcasts,
            memory,
            now);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("1", result[0].Id);
    }

    [TestMethod]
    public void FindNewBroadcasts_IncludesDelayedBroadcast()
    {
        var now = DateTime.UtcNow;

        var memory = new BroadcastPublishedMemory
        {
            LastInteractionDate = now.AddMinutes(-2)
        };

        var broadcasts = new List<BroadcastDto>
    {
        new()
        {
            Id = "delayed-1",
            PublishedAt = now.AddMinutes(-4)
        }
    };

        var result = BroadcastPollingList.FindNewBroadcasts(
            broadcasts,
            memory,
            now);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("delayed-1", result[0].Id);
    }

    [TestMethod]
    public void CleanupMemory_RemovesExpiredBroadcasts()
    {
        var now = DateTime.UtcNow;

        var memory = new BroadcastPublishedMemory
        {
            LastInteractionDate = now,
            KnownBroadcastIds = new Dictionary<string, DateTime>
        {
            { "old-1", now.AddMinutes(-10) },
            { "old-2", now.AddMinutes(-6) },
            { "recent-1", now.AddMinutes(-3) },
            { "recent-2", now.AddMinutes(-1) }
        }
        };

        BroadcastPollingList.CleanupMemory(memory);

        Assert.AreEqual(2, memory.KnownBroadcastIds.Count);
        Assert.IsFalse(memory.KnownBroadcastIds.ContainsKey("old-1"));
        Assert.IsFalse(memory.KnownBroadcastIds.ContainsKey("old-2"));
        Assert.IsTrue(memory.KnownBroadcastIds.ContainsKey("recent-1"));
        Assert.IsTrue(memory.KnownBroadcastIds.ContainsKey("recent-2"));
    }

    [TestMethod]
    [TestCategory("Manual")]
    public async Task OnBroadcastPublished_ShouldPollKit()
    {
        var pollingList = new BroadcastPollingList(InvocationContext);

        var memory = new BroadcastPublishedMemory
        {
            LastInteractionDate = new DateTime(2026, 10, 7, 15, 10, 0, DateTimeKind.Utc)
        };

        var request = new PollingEventRequest<BroadcastPublishedMemory>
        {
            Memory = memory
        };

        var result = await pollingList.OnBroadcastPublished(request);
        var secondRequest = new PollingEventRequest<BroadcastPublishedMemory>
        {
            Memory = result.Memory
        };

        var secondResult = await pollingList.OnBroadcastPublished(secondRequest);

        Assert.IsFalse(secondResult.FlyBird);
        Assert.IsNotNull(secondResult.Result);
        Assert.AreEqual(0, secondResult.Result.Count);

        Console.WriteLine($"Second poll FlyBird: {secondResult.FlyBird}");
        Console.WriteLine($"Second poll broadcasts: {secondResult.Result.Count}");

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Memory);
        Assert.IsNotNull(result.Result);

        Console.WriteLine($"FlyBird: {result.FlyBird}");
        Console.WriteLine($"Broadcasts detected: {result.Result.Count}");
        Console.WriteLine(
            JsonConvert.SerializeObject(result.Result, Formatting.Indented));
    }
}
