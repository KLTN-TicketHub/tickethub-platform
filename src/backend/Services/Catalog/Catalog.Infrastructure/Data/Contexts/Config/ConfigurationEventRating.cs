using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationEventRating : IEntityTypeConfiguration<EventRating>
    {
        public void Configure(EntityTypeBuilder<EventRating> builder)
        {
            builder.ToTable("EventRating");

            builder.HasKey(er => er.Id);

            builder.HasOne(er => er.Event)
                .WithMany(e => e.EventRatings)
                .HasForeignKey(er => er.EventId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.Property(er => er.UserId)
                .IsRequired();

            builder.Property(er => er.ReviewerName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(er => er.SoundRating)
                .IsRequired();

            builder.Property(er => er.VisualRating)
                .IsRequired();

            builder.Property(er => er.OrganizationRating)
                .IsRequired();

            builder.Property(er => er.FacilityRating)
                .IsRequired();

            builder.Property(er => er.ServiceRating)
                .IsRequired();

            builder.Property(er => er.PerformanceRating)
                .IsRequired();

            builder.Property(er => er.OverallRating)
                .IsRequired();

            builder.Property(er => er.Comment)
                .HasMaxLength(1000);

            builder.Property(er => er.RowVersion)
                .IsRowVersion();

            builder.HasIndex(er => new { er.EventId, er.UserId })
                .IsUnique();
        }
    }
}
