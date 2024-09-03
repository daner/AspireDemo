using AspireDemo.Api.IntegrationTests.Setup;
using AspireDemo.Api.Messages;
using FluentAssertions;
using System.Net;

namespace AspireDemo.Api.IntegrationTests;

public class MessageTests : DatabaseTestFixture
{
    [Fact]
    public async Task GetMessagesForEmptyRoom()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/message/test");

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var list = await response.Content.ReadFromJsonAsync<List<MessageDto>>();

        list.Should().NotBeNull();
        list.Should().HaveCount(0);
    }

    [Fact]
    public async Task PostMessageToRoom()
    {
        var client = _factory.CreateClient();

        var text = "Message content";
        var message = new CreateMessage(text);

        await client.PostAsJsonAsync("/api/message/test", message);

        var response = await client.GetAsync("/api/message/test");

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var list = await response.Content.ReadFromJsonAsync<List<MessageDto>>();

        list.Should().NotBeNull();
        list.Should().HaveCount(1);
        list!.First().Text.Should().Be(text);
    }
}
