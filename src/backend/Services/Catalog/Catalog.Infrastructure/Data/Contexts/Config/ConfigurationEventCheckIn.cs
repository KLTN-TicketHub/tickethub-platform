using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationEventCheckIn : IEntityTypeConfiguration<EventCheckIn>
    {
        public void Configure(EntityTypeBuilder<EventCheckIn> builder)
        {
            builder.ToTable("EventCheckIn");

            builder.HasKey(c => c.Id);

            builder.HasOne<Event>()
                .WithMany()
                .HasForeignKey(c => c.EventId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.Property(c => c.UserId)
                .IsRequired();

            builder.Property(c => c.IssuedTicketId)
                .IsRequired();

            builder.Property(c => c.CheckedInAt)
                .IsRequired();

            builder.HasIndex(c => c.IssuedTicketId)
                .IsUnique();
        }
    }
}
