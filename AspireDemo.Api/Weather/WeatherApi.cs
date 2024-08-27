using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace AspireDemo.Api.Weather;

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public static class WeatherApi
{
    public static IEndpointRouteBuilder MapWeatherApi(this IEndpointRouteBuilder builder)
    {
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        builder.MapGet("/api/weatherforecast", () =>
            {
                var forecast = Enumerable.Range(1, 5).Select(index =>
                        new WeatherForecast
                        (
                            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                            Random.Shared.Next(-20, 55),
                            summaries[Random.Shared.Next(summaries.Length)]
                        ))
                    .ToArray();
                return forecast;
            })
            .WithName("GetWeatherForecast")
            .WithOpenApi();

        builder.MapGet("/api/weatherforecast/{countryCode}/{city}", async
            ([FromServices] IOptions<OpenWeatherMapOptions> options,
                [FromServices] IHttpClientFactory clientFactory,
                [FromServices] IConnectionMultiplexer connectionMultiplexer,
                [FromRoute] string countryCode,
                [FromRoute] string city) =>
            {
                var redis = connectionMultiplexer.GetDatabase();

                var keyName = $"forecast:{city}:{countryCode}";
                var json = await redis.StringGetAsync(keyName);

                if (string.IsNullOrEmpty(json))
                {
                    var client = clientFactory.CreateClient();
                    var result =
                        await client.GetAsync(
                            $"{options.Value.Url}?q={city},{countryCode}&APPID={options.Value.Key}&units=metric");
                    json = await result.Content.ReadAsStringAsync();
                    var setTask = redis.StringSetAsync(keyName, json);
                    var expireTask = redis.KeyExpireAsync(keyName, TimeSpan.FromSeconds(120));
                    await Task.WhenAll(setTask, expireTask);
                }

                return Results.Content(json, contentType: "application/json", statusCode: 200);
            })
            .WithName("SearchWeather")
            .WithOpenApi()
            .RequireAuthorization("User");

        return builder;
    }
}