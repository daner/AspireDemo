using AspireDemo.Data.Entities;
using AspireDemo.Data.Mappings;
using Microsoft.EntityFrameworkCore;

namespace AspireDemo.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> optisons) : DbContext(optisons)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new MessageMap());
    }
    
    public DbSet<Message> Messages { get; set; }
}