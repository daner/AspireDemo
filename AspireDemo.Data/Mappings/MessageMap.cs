using AspireDemo.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AspireDemo.Data.Mappings;

public class MessageMap : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages", "AspireDemo");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .IsRequired();

        builder.Property(m => m.Timestamp)
            .IsRequired();
        
        builder.Property(m => m.Username)
            .HasMaxLength(500)
            .IsRequired();
        
        builder.Property(m => m.Room)
            .HasMaxLength(500)
            .IsRequired();
        
        builder.Property(m => m.Text)
            .HasMaxLength(3000)
            .IsRequired();
    }
}