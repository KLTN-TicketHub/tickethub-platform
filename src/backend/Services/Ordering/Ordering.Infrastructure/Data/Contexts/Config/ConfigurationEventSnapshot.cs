using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Infrastructure.Entities;

namespace Ordering.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationEventSnapshot : IEntityTypeConfiguration<EventSnapshot>
    {
        public void Configure(EntityTypeBuilder<EventSnapshot> builder)
        {
            builder.ToTable("EventSnapshot");

            builder.HasKey(e => e.Id);

            builder.HasIndex(e => e.EventId).IsUnique();

            builder.Property(e => e.EventId).IsRequired();

            builder.Property(e => e.EventTitle)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(e => e.EventImage)
                .HasMaxLength(2000);

            builder.Property(e => e.CategoryId).IsRequired();

            builder.Property(e => e.CategoryName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.OrganizerId).IsRequired();

            builder.Property(e => e.OrganizerName)
                .HasMaxLength(300)
                .IsRequired();

            builder.Property(e => e.SaleOpenAt).IsRequired();
            builder.Property(e => e.SaleCloseAt).IsRequired();
            builder.Property(e => e.SnapshotCreatedAt).IsRequired();

            builder.HasMany(e => e.Showtimes)
                .WithOne(s => s.EventSnapshot)
                .HasForeignKey(s => s.EventSnapshotId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
