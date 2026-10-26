using Apps.Kit.Models.Dtos;
using Apps.Kit.Polling;
using Apps.Kit.Polling.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Kit.Polling;

[TestClass]
public class SubscriberPollingTests
{
    [TestMethod]
    public void FindsNewSubscribersWithinPollingWindow()
    {
        var checkpoint = new DateTime(
            2026, 10, 9, 12, 0, 0, DateTimeKind.Utc);

        var memory = new SubscriberCreatedMemory
        {
            LastInteractionDate = checkpoint
        };

        var subscribers = new List<SubscriberDto>
        {
            new()
            {
                Id = "1001",
                EmailAddress = "new@example.com",
                CreatedAt = checkpoint.AddMinutes(2)
            },
            new()
            {
                Id = "1002",
                EmailAddress = "old@example.com",
                CreatedAt = checkpoint.AddMinutes(-10)
            }
        };

        var result = SubscriberPollingList.FindNewSubscribers(
            subscribers,
            memory,
            checkpoint.AddMinutes(5));

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("1001", result[0].Id);
    }

    [TestMethod]
    public void DoesNotTriggerKnownSubscribersAgain()
    {
        var checkpoint = new DateTime(
            2026, 10, 9, 12, 0, 0, DateTimeKind.Utc);

        var memory = new SubscriberCreatedMemory
        {
            LastInteractionDate = checkpoint,
            KnownSubscriberIds = new Dictionary<string, DateTime>
            {
                ["1001"] = checkpoint.AddMinutes(1)
            }
        };

        var subscribers = new List<SubscriberDto>
        {
            new()
            {
                Id = "1001",
                CreatedAt = checkpoint.AddMinutes(1)
            }
        };

        var result = SubscriberPollingList.FindNewSubscribers(
            subscribers,
            memory,
            checkpoint.AddMinutes(5));

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void RemovesExpiredSubscriberIds()
    {
        var checkpoint = new DateTime(
            2026, 10, 9, 12, 0, 0, DateTimeKind.Utc);

        var memory = new SubscriberCreatedMemory
        {
            LastInteractionDate = checkpoint,
            KnownSubscriberIds = new Dictionary<string, DateTime>
            {
                ["old"] = checkpoint.AddMinutes(-10),
                ["recent"] = checkpoint.AddMinutes(-2)
            }
        };

        SubscriberPollingList.CleanupMemory(memory);

        Assert.IsFalse(memory.KnownSubscriberIds.ContainsKey("old"));
        Assert.IsTrue(memory.KnownSubscriberIds.ContainsKey("recent"));
    }

    [TestMethod]
    public void FindsNewUnsubscribersWithinPollingWindow()
    {
        var checkpoint = new DateTime(
            2026, 10, 9, 12, 0, 0, DateTimeKind.Utc);

        var memory = new SubscriberUnsubscribedMemory
        {
            LastInteractionDate = checkpoint
        };

        var subscribers = new List<SubscriberDto>
    {
        new()
        {
            Id = "2001",
            EmailAddress = "unsubscribed@example.com",
            CanceledAt = checkpoint.AddMinutes(2)
        },
        new()
        {
            Id = "2002",
            EmailAddress = "previously@example.com",
            CanceledAt = checkpoint.AddMinutes(-10)
        }
    };

        var result = SubscriberPollingList.FindNewUnsubscribers(
            subscribers,
            memory,
            checkpoint.AddMinutes(5));

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("2001", result[0].Id);
    }

    [TestMethod]
    public void DoesNotTriggerKnownUnsubscribersAgain()
    {
        var checkpoint = new DateTime(
            2026, 10, 9, 12, 0, 0, DateTimeKind.Utc);

        var memory = new SubscriberUnsubscribedMemory
        {
            LastInteractionDate = checkpoint,
            KnownSubscriberIds = new Dictionary<string, DateTime>
            {
                ["2001"] = checkpoint.AddMinutes(1)
            }
        };

        var subscribers = new List<SubscriberDto>
    {
        new()
        {
            Id = "2001",
            CanceledAt = checkpoint.AddMinutes(1)
        }
    };

        var result = SubscriberPollingList.FindNewUnsubscribers(
            subscribers,
            memory,
            checkpoint.AddMinutes(5));

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void IgnoresUnsubscribersAfterCurrentCheckpoint()
    {
        var checkpoint = new DateTime(
            2026, 10, 9, 12, 0, 0, DateTimeKind.Utc);

        var memory = new SubscriberUnsubscribedMemory
        {
            LastInteractionDate = checkpoint
        };

        var subscribers = new List<SubscriberDto>
    {
        new()
        {
            Id = "2003",
            CanceledAt = checkpoint.AddMinutes(10)
        }
    };

        var result = SubscriberPollingList.FindNewUnsubscribers(
            subscribers,
            memory,
            checkpoint.AddMinutes(5));

        Assert.AreEqual(0, result.Count);
    }
}
