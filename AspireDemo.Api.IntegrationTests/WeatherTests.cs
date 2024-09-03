using FluentAssertions;
using System.Net;
using AspireDemo.Api.IntegrationTests.Setup;
using System.Net.Http.Headers;

namespace AspireDemo.Api.IntegrationTests;

public class WeatherTests : RedisTestFixture
{
    [Fact]
    public async Task GetWeather()
    {
        var client = _factory.CreateClient();

        var weather = await client.GetAsync("/api/weatherforecast");

        weather.Should().NotBeNull();
        weather.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetWeatherForLocation()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");

        var weather = await client.GetAsync("/api/weatherforecast/uk/london");

        weather.Should().NotBeNull();
        weather.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}