using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationEventCancellationRequest : IEntityTypeConfiguration<EventCancellationRequest>
    {
        public void Configure(EntityTypeBuilder<EventCancellationRequest> builder)
        {
            builder.ToTable("EventCancellationRequest");

            builder.HasKey(r => r.Id);

            builder.HasOne<Event>()
                .WithMany()
                .HasForeignKey(r => r.EventId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.Property(r => r.EventTitle)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(r => r.Reason)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(r => r.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(r => r.ReviewerName)
                .HasMaxLength(200);

            builder.Property(r => r.RejectionReason)
                .HasMaxLength(500);
        }
    }
}
