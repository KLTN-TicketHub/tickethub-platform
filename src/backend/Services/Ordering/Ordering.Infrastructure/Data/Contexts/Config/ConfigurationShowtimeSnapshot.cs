using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Infrastructure.Entities;

namespace Ordering.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationShowtimeSnapshot : IEntityTypeConfiguration<ShowtimeSnapshot>
    {
        public void Configure(EntityTypeBuilder<ShowtimeSnapshot> builder)
        {
            builder.ToTable("ShowtimeSnapshot");

            builder.HasKey(s => s.Id);

            builder.HasIndex(s => s.ShowtimeId).IsUnique();

            builder.Property(s => s.EventSnapshotId).IsRequired();
            builder.Property(s => s.ShowtimeId).IsRequired();
            builder.Property(s => s.StartAt).IsRequired();
            builder.Property(s => s.EndAt).IsRequired();

            builder.HasMany(s => s.TicketTypes)
                .WithOne(t => t.ShowtimeSnapshot)
                .HasForeignKey(t => t.ShowtimeSnapshotId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
