namespace AspireDemo.Data.Entities;

public class Message
{
    public int Id { get; set; }
    public required string Sender { get; set; }
    public required string Receiever { get; set; }
    public required string Text { get; set; }
}