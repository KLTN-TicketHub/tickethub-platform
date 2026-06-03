using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationEventApproval : IEntityTypeConfiguration<EventApproval>
    {
        public void Configure(EntityTypeBuilder<EventApproval> builder)
        {
            builder.ToTable("EventApproval");

            builder.HasKey(ea => ea.Id);

            builder.HasOne(ea => ea.Event)
                .WithMany()
                .HasForeignKey(ea => ea.EventId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.Property(ea => ea.ApprovalStatus)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(ea => ea.Reason)
                .HasMaxLength(500);

            builder.Property(ea => ea.ReviewerUserId)
                .HasMaxLength(100);

            builder.Property(ea => ea.ReviewerName)
                .HasMaxLength(200);
        }
    }
}