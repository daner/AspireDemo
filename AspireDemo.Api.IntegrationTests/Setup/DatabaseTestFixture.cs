using Testcontainers.MsSql;
using Testcontainers.Redis;

namespace AspireDemo.Api.IntegrationTests.Setup;

public class DatabaseTestFixture : IAsyncLifetime
{
    protected ApiWebApplicationFactory _factory = default!;
    private readonly MsSqlContainer _databaseContainer = new MsSqlBuilder().Build();

    public async Task InitializeAsync()
    {
        await _databaseContainer.StartAsync();

        _factory = new ApiWebApplicationFactory() 
        { 
            DbConnectionString = _databaseContainer.GetConnectionString() 
        };
    }

    public async Task DisposeAsync()
    {
        await _databaseContainer.DisposeAsync();
        await _factory.DisposeAsync();
    }
}
