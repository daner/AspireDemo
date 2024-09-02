using System.ComponentModel.DataAnnotations;

namespace AspireDemo.EmailWorker;

public class EmailOptions
{
    [Required]
    public required string From { get; set; }
}
