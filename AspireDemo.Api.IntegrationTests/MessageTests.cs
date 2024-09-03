using AspireDemo.Api.IntegrationTests.Setup;
using FluentAssertions;
using System.Net;

namespace AspireDemo.Api.IntegrationTests;

public class MessageTests : DatabaseTestFixture
{
    [Fact]
    public async Task GetMessagesForEmptyRoom()
    {
        var client = _factory.CreateClient();

        var messages = await client.GetAsync("/api/message/test");

        messages.Should().NotBeNull();
        messages.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
