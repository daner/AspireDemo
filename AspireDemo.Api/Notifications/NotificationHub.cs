using AspireDemo.Api.Messages;
using Microsoft.AspNetCore.SignalR;

namespace AspireDemo.Api.Notifications;

public class NotificationHub(ILogger<NotificationHub> logger) : Hub
{
    public const string ChatMessage = "ChatMessage";

    public async Task JoinRoom(string roomName)
    {
        logger.LogInformation("User {User} with connectionId {Connectionid} joined room {roomName}",
            Context.User?.Identity?.Name, Context.ConnectionId, roomName);
        
        
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);

        var message = new MessageDto(-1, DateTimeOffset.Now.ToString("HH:mm"), "System", roomName, $"{Context.User?.Identity?.Name ?? "Unknown"} joined room.");

        await Clients.Group(roomName).SendAsync(ChatMessage, message);        
    }

    public async Task LeaveRoom(string roomName)
    {
        logger.LogInformation("User {user} with connectionId {Connectionid} left room {roomName}",
            Context.User?.Identity?.Name, Context.ConnectionId, roomName);

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);

        var message = new MessageDto(-1, DateTimeOffset.Now.ToString("HH:mm"), "System", roomName, $"{Context.User?.Identity?.Name ?? "Unknown"} left room.");

        await Clients.Group(roomName).SendAsync(ChatMessage, message);
    }
}