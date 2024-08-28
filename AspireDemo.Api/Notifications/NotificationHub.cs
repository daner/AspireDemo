using Microsoft.AspNetCore.SignalR;

namespace AspireDemo.Api.Notifications;

public class NotificationHub(ILogger<NotificationHub> logger) : Hub
{
    public Task JoinRoom(string roomName)
    {
        logger.LogInformation("User {User} with connectionId {Connectionid} joining room {roomName}",
            Context.User?.Identity?.Name, Context.ConnectionId, roomName);
        
        return Groups.AddToGroupAsync(Context.ConnectionId, roomName);
    }

    public Task LeaveRoom(string roomName)
    {
        logger.LogInformation("User {user} with connectionId {Connectionid} leaving room {roomName}",
            Context.User?.Identity?.Name, Context.ConnectionId, roomName);
        
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
    }
}