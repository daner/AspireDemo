namespace AspireDemo.Data.Entities;

public class Message
{
    public int Id { get; set; }
    public required DateTimeOffset Timestamp { get; set; }
    public required string Username { get; set; }
    public required string Room { get; set; }
    public required string Text { get; set; }
}