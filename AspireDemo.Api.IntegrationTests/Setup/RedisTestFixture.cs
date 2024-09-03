using Testcontainers.Redis;

namespace AspireDemo.Api.IntegrationTests.Setup;

public class RedisTestFixture : IAsyncLifetime
{
    protected ApiWebApplicationFactory _factory = default!;
    private readonly RedisContainer _redisContainer = new RedisBuilder().Build();

    public async Task InitializeAsync()
    {
        await _redisContainer.StartAsync();

        _factory = new ApiWebApplicationFactory() 
        { 
            RedisConnectionString = _redisContainer.GetConnectionString() 
        };
    }

    public async Task DisposeAsync()
    {
        await _redisContainer.DisposeAsync();
        await _factory.DisposeAsync();
    }
}
