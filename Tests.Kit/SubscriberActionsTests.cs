using Apps.Kit.Actions;
using Apps.Kit.Constants;
using Apps.Kit.DataSourceHandlers;
using Apps.Kit.Models.Requests;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Newtonsoft.Json;
using Tests.Kit.Base;

namespace Tests.Kit;

[TestClass]
public class SubscriberActionsTests : TestBase
{
    private const string SubscriberId = "4309128168";
    private const string TagId = "23259003";

    [TestMethod]
    public async Task SearchSubscribers_ShouldReturnSubscribers()
    {
        var actions = new SubscriberActions(InvocationContext);
        var request = new SearchSubscribersRequest
        {
            Status = SubscriberStates.Active,
            EmailAddress = "tobetranslated01@gmail.com",
            CreatedAfter = new DateTime(2026, 9, 15)
        };

        var result = await actions.SearchSubscribers(request);

        Assert.IsTrue(result.Subscribers.Count > 0);
        Assert.AreEqual(request.EmailAddress, result.Subscribers.First().EmailAddress);
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }

    [TestMethod]
    public async Task GetSubscriber_ValidId_ShouldReturnSubscriber()
    {
        var actions = new SubscriberActions(InvocationContext);

        var result = await actions.GetSubscriber(new SubscriberRequest { SubscriberId = SubscriberId });

        Assert.AreEqual(SubscriberId, result.Id);
        Assert.IsFalse(string.IsNullOrEmpty(result.EmailAddress));
    }

    [TestMethod]
    public async Task GetSubscriberTags_ValidInput_ShouldReturnTags()
    {
        var actions = new SubscriberActions(InvocationContext);

        var result = await actions.GetSubscriberTags(new SubscriberRequest { SubscriberId = "4102640914" });

        Assert.IsNotNull(result.Tags);
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }

    [TestMethod]
    public async Task GetSubscriberStats_ValidInput_ShouldReturnStats()
    {
        var actions = new SubscriberActions(InvocationContext);
        var request = new GetSubscriberStatsRequest
        {
            SubscriberId = "4102640914",
            SentAfter = new DateTime(2025, 1, 1),
            SentBefore = new DateTime(2026, 10, 3)
        };

        var result = await actions.GetSubscriberStats(request);

        Assert.AreEqual(request.SubscriberId, result.Id);
        Assert.IsNotNull(result.Stats);
    }

    [TestMethod]
    public async Task CreateSubscriber_ValidInput_ShouldCreateSubscriber()
    {
        var actions = new SubscriberActions(InvocationContext);
        var request = new CreateSubscriberRequest
        {
            EmailAddress = "danielmilczarski@aol.com",
            FirstName = "Blackbird Test Updated",
            State = SubscriberStates.Inactive
        };

        var result = await actions.CreateSubscriber(request);

        Assert.AreEqual(request.EmailAddress, result.EmailAddress);
    }

    [TestMethod]
    public async Task AddTagToSubscriber_ValidInput_ShouldSucceed()
    {
        var actions = new SubscriberActions(InvocationContext);

        var result = await actions.AddTagToSubscriber(new TagSubscriberRequest { SubscriberId = SubscriberId, TagId = TagId });

        Assert.AreEqual(SubscriberId, result.Id);
    }

    [TestMethod]
    public async Task DeleteTagFromSubscriber_ValidInput_ShouldSucceed()
    {
        var actions = new SubscriberActions(InvocationContext);

        await actions.DeleteTagFromSubscriber(new TagSubscriberRequest { SubscriberId = SubscriberId, TagId = TagId });
    }

    [TestMethod]
    public async Task UpdateSubscriber_ValidInput_ShouldUpdateSubscriber()
    {
        var actions = new SubscriberActions(InvocationContext);
        var request = new UpdateSubscriberRequest
        {
            SubscriberId = SubscriberId,
            EmailAddress = "danielmilczarski@aol.com",
            FirstName = "Blackbird Test Updated"
        };

        var result = await actions.UpdateSubscriber(request);

        Assert.AreEqual(request.EmailAddress, result.EmailAddress);
        Assert.AreEqual(request.FirstName, result.FirstName);
    }

    [TestMethod]
    public async Task UpdateSubscriber_WithoutEmail_ShouldKeepEmail()
    {
        var actions = new SubscriberActions(InvocationContext);
        var request = new UpdateSubscriberRequest { SubscriberId = SubscriberId, FirstName = "Blackbird No Email Test" };

        var result = await actions.UpdateSubscriber(request);

        Assert.AreEqual(request.FirstName, result.FirstName);
        Assert.IsFalse(string.IsNullOrEmpty(result.EmailAddress));
    }

    [TestMethod]
    public async Task UpdateSubscriberCustomField_ValidInput_ShouldUpdateCustomField()
    {
        var actions = new SubscriberActions(InvocationContext);
        var request = new UpdateSubscriberCustomFieldRequest
        {
            SubscriberId = SubscriberId,
            CustomField = "role",
            Value = "Solutions"
        };

        var result = await actions.UpdateSubscriberCustomField(request);

        Assert.IsNotNull(result.Fields);
        Assert.AreEqual(request.Value, result.Fields[request.CustomField]);
    }

    [TestMethod]
    public async Task UnsubscribeSubscriber_ValidInput_ShouldSucceed()
    {
        var actions = new SubscriberActions(InvocationContext);

        await actions.UnsubscribeSubscriber(new SubscriberRequest { SubscriberId = "4309121579" });
    }

    [TestMethod]
    public async Task CustomFieldDataHandler_ShouldReturnCustomFields()
    {
        var handler = new CustomFieldDataHandler(InvocationContext);

        var result = (await handler.GetDataAsync(new DataSourceContext(), CancellationToken.None)).ToList();

        Assert.IsTrue(result.Count > 0);
    }

    [TestMethod]
    public async Task TagDataHandler_ShouldReturnTags()
    {
        var handler = new TagDataHandler(InvocationContext);

        var result = (await handler.GetDataAsync(new DataSourceContext(), CancellationToken.None)).ToList();

        Assert.IsTrue(result.Count > 0);
    }
}
