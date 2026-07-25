using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationEventCancellation : IEntityTypeConfiguration<EventCancellation>
    {
        public void Configure(EntityTypeBuilder<EventCancellation> builder)
        {
            builder.ToTable("EventCancellation");

            builder.HasKey(ec => ec.Id);

            builder.HasOne(ec => ec.Event)
                .WithOne(e => e.Cancellation)
                .HasForeignKey<EventCancellation>(ec => ec.EventId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.Property(ec => ec.Reason)
                .HasMaxLength(500);

            builder.Property(ec => ec.CancelledByUserId)
                .IsRequired();

            builder.Property(ec => ec.CancelledByName)
                .HasMaxLength(200);
        }
    }
}
