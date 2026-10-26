using Apps.Kit.Actions;
using Apps.Kit.Models.Requests;
using Newtonsoft.Json;
using Tests.Kit.Base;
using Apps.Kit.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Tests.Kit;

[TestClass]
public class SubscriberActionsTests : TestBase
{
    [TestMethod]
    public async Task GetSubscriber_ValidId_ShouldReturnSubscriber()
    {
        var subscriberActions = new SubscriberActions(InvocationContext);
        var request = new SubscriberRequest
        {
            SubscriberId = "4309128168"
        };

        var result = await subscriberActions.GetSubscriber(request);

        Assert.IsNotNull(result);
        Assert.AreEqual(request.SubscriberId, result.Id);
        Assert.IsFalse(string.IsNullOrEmpty(result.EmailAddress));
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }

    [TestMethod]
    public async Task SearchSubscribers_ShouldReturnSubscribers()
    {
        var subscriberActions = new SubscriberActions(InvocationContext);

        var request = new SearchSubscribersRequest
        {
            Status = "active",
            EmailAddress = "tobetranslated01@gmail.com",
            CreatedAfter = new DateTime(2026, 9, 15)
        };

        var result = await subscriberActions.SearchSubscribers(request);

        Assert.IsNotNull(result.Subscribers);
        Assert.IsTrue(result.Subscribers.Count > 0);
        Assert.AreEqual(
            request.EmailAddress,
            result.Subscribers.First().EmailAddress);
        Assert.IsNotNull(result.Pagination);
        Console.WriteLine($"Has next page: {result.Pagination.HasNextPage}");
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        Console.WriteLine($"Total subscribers returned: {result.Subscribers.Count}");
    }

    [TestMethod]
    public async Task CreateSubscriber_ValidInput_ShouldCreateSubscriber()
    {
        var subscriberActions = new SubscriberActions(InvocationContext);

        var request = new CreateSubscriberRequest
        {
            EmailAddress = "danielmilczarski@aol.com",
            FirstName = "Blackbird Test Updated",
            State = "inactive"
        };

        var result = await subscriberActions.CreateSubscriber(request);
        Assert.IsNotNull(result);
        Assert.AreEqual(request.EmailAddress, result.EmailAddress);

        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }

    [TestMethod]
    public async Task UpdateSubscriber_ValidInput_ShouldUpdateSubscriber()
    {
        var subscriberActions = new SubscriberActions(InvocationContext);

        var request = new UpdateSubscriberRequest
        {
            SubscriberId = "4309128168",
            EmailAddress = "danielmilczarski@aol.com",
            FirstName = "Blackbird Test Updated"
        };

        var result = await subscriberActions.UpdateSubscriber(request);
        Assert.IsNotNull(result);
        Assert.AreEqual(request.SubscriberId, result.Id);
        Assert.AreEqual(request.EmailAddress, result.EmailAddress);
        Assert.AreEqual(request.FirstName, result.FirstName);

        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }

    [TestMethod]
    public async Task UpdateSubscriber_WithoutEmail_ShouldUpdateSubscriber()
    {
        var subscriberActions = new SubscriberActions(InvocationContext);

        var request = new UpdateSubscriberRequest
        {
            SubscriberId = "4309128168",
            FirstName = "Blackbird No Email Test"
        };

        var result = await subscriberActions.UpdateSubscriber(request);
        Assert.IsNotNull(result);
        Assert.AreEqual(request.SubscriberId, result.Id);
        Assert.AreEqual(request.FirstName, result.FirstName);
        Assert.IsFalse(string.IsNullOrEmpty(result.EmailAddress));

        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }

    [TestMethod]
    public async Task CustomFieldDataHandler_ShouldReturnCustomFields()
    {
        var handler = new CustomFieldDataHandler(InvocationContext);

        var result = await handler.GetDataAsync(
            new DataSourceContext(),
            CancellationToken.None);

        var fields = result.ToList();

        Assert.IsTrue(fields.Count > 0);

        foreach (var field in fields)
        {
            Console.WriteLine($"{field.DisplayName} -> {field.Value}");
        }
    }

    [TestMethod]
    public async Task UpdateSubscriberCustomField_ValidInput_ShouldUpdateCustomField()
    {
        var subscriberActions = new SubscriberActions(InvocationContext);

        var request = new UpdateSubscriberCustomFieldRequest
        {
            SubscriberId = "4309128168",
            CustomField = "role",
            Value = "Solutions",
        };

        var result = await subscriberActions.UpdateSubscriberCustomField(request);

        Assert.IsNotNull(result);
        Assert.AreEqual(request.SubscriberId, result.Id);
        Assert.IsNotNull(result.Fields);
        Assert.AreEqual(
            request.Value,
            result.Fields[request.CustomField]);

        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }

    [TestMethod]
    public async Task UnsubscribeSubscriber_ValidInput_ShouldSucceed()
    {
        var subscriberActions = new SubscriberActions(InvocationContext);

        var request = new SubscriberRequest
        {
            SubscriberId = "4309121579"
        };

        await subscriberActions.UnsubscribeSubscriber(request);
    }

    [TestMethod]
    public async Task TagDataHandler_ShouldReturnTags()
    {
        var handler = new TagDataHandler(InvocationContext);

        var result = await handler.GetDataAsync(
            new DataSourceContext(),
            CancellationToken.None);

        var tags = result.ToList();

        Assert.IsTrue(tags.Count > 0);

        foreach (var tag in tags)
        {
            Console.WriteLine($"{tag.DisplayName} -> {tag.Value}");
        }
    }

    [TestMethod]
    public async Task TagSubscriber_ValidInput_ShouldSucceed()
    {
        var subscriberActions = new SubscriberActions(InvocationContext);

        var request = new TagSubscriberRequest
        {
            SubscriberId = "4309128168",
            TagId = "23259003"
        };

        var result = await subscriberActions.TagSubscriber(request);

        Assert.IsNotNull(result);
        Assert.AreEqual(request.SubscriberId, result.Id);
    }

    [TestMethod]
    public async Task GetSubscriberStats_ValidInput_ShouldReturnStats()
    {
        var subscriberActions = new SubscriberActions(InvocationContext);

        var request = new GetSubscriberStatsRequest
        {
            SubscriberId = "4102640914",
            SentAfter = new DateTime(2025, 1, 1),
            SentBefore = new DateTime(2026, 10, 3)
        };

        var result = await subscriberActions.GetSubscriberStats(request);

        Assert.IsNotNull(result);
        Assert.AreEqual(request.SubscriberId, result.Id);
        Assert.IsNotNull(result.Stats);

        Console.WriteLine($"Sent: {result.Stats.Sent}");
        Console.WriteLine($"Opened: {result.Stats.Opened}");
        Console.WriteLine($"Clicked: {result.Stats.Clicked}");
        Console.WriteLine($"Open rate: {result.Stats.OpenRate}");
        Console.WriteLine($"Click rate: {result.Stats.ClickRate}");
    }

    [TestMethod]
    public async Task GetSubscriberTags_ValidInput_ShouldReturnTags()
    {
        var subscriberActions = new SubscriberActions(InvocationContext);

        var request = new SubscriberRequest
        {
            SubscriberId = "4102640914"
        };

        var result = await subscriberActions.GetSubscriberTags(request);

        Assert.IsNotNull(result);

        foreach (var tag in result)
        {
            Console.WriteLine($"{tag.Id}: {tag.Name} - Tagged at: {tag.TaggedAt}");
        }
    }

    [TestMethod]
    public async Task RemoveTagFromSubscriber_ValidInput_ShouldSucceed()
    {
        var subscriberActions = new SubscriberActions(InvocationContext);

        var request = new TagSubscriberRequest
        {
            SubscriberId = "4309128168",
            TagId = "23259003"
        };

        await subscriberActions.RemoveTagFromSubscriber(request);
    }
}
