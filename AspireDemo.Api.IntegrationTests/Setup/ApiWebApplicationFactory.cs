using AspireDemo.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;

namespace AspireDemo.Api.IntegrationTests.Setup;

public class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    public string RedisConnectionString { get; set; } = "notused";
    public string DbConnectionString { get; set; } = "notused";
    public string DefaultUserId { get; set; } = "1";

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(configure =>
        {
            configure.AddInMemoryCollection([
                new("ConnectionStrings:seq", "http://notused"),
                new("ConnectionStrings:cache", RedisConnectionString),
                new("ConnectionStrings:aspiredemo", DbConnectionString)
            ]);
        });

        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.Configure<TestAuthHandlerOptions>(options => options.DefaultUserId = DefaultUserId);

            services.AddAuthentication(TestAuthHandler.AuthenticationScheme)
                .AddScheme<TestAuthHandlerOptions, TestAuthHandler>(TestAuthHandler.AuthenticationScheme, options => { });


            if (DbConnectionString != "notused")
            {
                services.EnsureDbCreated<ApplicationDbContext>();
            }
        });
    }
}

public static class ServiceCollectionExtensions
{
    public static void EnsureDbCreated<T>(this IServiceCollection services) where T : DbContext
    {
        var serviceProvider = services.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var context = scopedServices.GetRequiredService<T>();
        context.Database.EnsureCreated();
    }
}