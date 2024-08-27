namespace AspireDemo.Api.Messages;

public record MessageDto(int Id, string Timestamp, string Username, string Room, string Text);

public record CreateMessage(string Text);