using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace AspireDemo.Api.Email;

public record EmailMessage(string From, string To, string Body);

public static class EmailApi
{
    public static RouteGroupBuilder MapEmailApi(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/email");

        group.MapPost("/", ([FromServices] IConnection rabbitConnection, [FromServices] IHttpContextAccessor context) =>
        {

            var to = context.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

            if (to == null)
            {
                return Results.BadRequest();
            }

            var messsage = new EmailMessage("noreply@aspire.demo", to, "Hello World!");
            var jsonString = JsonSerializer.Serialize(messsage);

            var body = Encoding.UTF8.GetBytes(jsonString);

            using var channel = rabbitConnection.CreateModel();
            channel.QueueDeclare(queue: "email", durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.BasicPublish(exchange: string.Empty, routingKey: "email", basicProperties: null, body: body);

            return Results.Ok();
        });

        return group;
    }
}
