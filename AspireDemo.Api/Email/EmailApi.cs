using AspireDemo.Api.Messaging;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AspireDemo.Api.Email;

public record EmailMessage(string From, string To, string Subject, string Body);

public static class EmailApi
{
    public static RouteGroupBuilder MapEmailApi(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/email");

        group.MapPost("/", ([FromServices] IMessageSender<EmailMessage> messageSender, [FromServices] IHttpContextAccessor context) =>
        {

            var to = context.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

            if (to == null)
            {
                return Results.BadRequest();
            }

            var messsage = new EmailMessage("noreply@aspire.demo", to, "Test", "Hello World!");
            
            messageSender.SendMessage(messsage);

            return Results.Ok();
        });

        return group;
    }

}
