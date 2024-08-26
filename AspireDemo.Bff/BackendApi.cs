using Microsoft.AspNetCore.Authentication;
using Yarp.ReverseProxy.Forwarder;
using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

namespace AspireDemo.Bff;

public static class BackendApi
{
    public static RouteGroupBuilder MapBackendApi(this IEndpointRouteBuilder builder, IConfiguration configuration)
    {
        var group = builder.MapGroup("/api");
        var backendUrl = configuration["services:api:http:0"] ?? "";
        
        var transformBuilder = builder.ServiceProvider.GetRequiredService<ITransformBuilder>();
        var transform = transformBuilder.Create(b =>
        {
            b.AddRequestTransform(async context =>
            {
                if (context.HttpContext.User.Identity?.IsAuthenticated ?? false)
                {
                    Console.WriteLine("Hello!");
                    var accessToken = await context.HttpContext.GetTokenAsync("access_token");
                    context.ProxyRequest.Headers.Authorization = new("Bearer", accessToken);    
                }
            });
        });
        
        group.MapForwarder("{*path}", backendUrl, new ForwarderRequestConfig(), transform);

        return group;
    }
}