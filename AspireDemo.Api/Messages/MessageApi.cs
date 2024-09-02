using AspireDemo.Api.Notifications;
using AspireDemo.Data;
using AspireDemo.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AspireDemo.Api.Messages;

public static class MessageApi
{
    public static RouteGroupBuilder MapMessageApi(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/message");

        group.MapGet("/{room}",
                async ([FromRoute] string room, [FromServices] ApplicationDbContext dbContext) =>
                {
                    var messages = await dbContext.Messages.Where(m => m.Room == room)
                        .OrderByDescending(m => m.Timestamp)
                        .ToListAsync();
                    return Results.Ok(messages.ToDto());
                })
            .WithOpenApi()
            .RequireAuthorization("User");

        group.MapPost("/{room}", async (
                [FromRoute] string room,
                [FromBody] CreateMessage body,
                [FromServices] IHttpContextAccessor context,
                [FromServices] IHubContext<NotificationHub> hub,
                [FromServices] ApplicationDbContext dbContext) =>
            {
                var username = context.HttpContext?.User.Identity?.Name ?? "Anonymous";
                var message = new Message()
                {
                    Room = room,
                    Timestamp = DateTimeOffset.Now,
                    Username = username,
                    Text = body.Text
                };

                dbContext.Messages.Add(message);
                await dbContext.SaveChangesAsync();
                
                var dto = message.ToDto();
                
                await hub.Clients.Groups(room).SendAsync(NotificationHub.ChatMessage, dto);
                
                return Results.Ok(dto);
            })
            .WithOpenApi()
            .RequireAuthorization("User");

        return group;
    }
}