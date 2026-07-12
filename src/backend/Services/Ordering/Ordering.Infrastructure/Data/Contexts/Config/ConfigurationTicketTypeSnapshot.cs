using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Infrastructure.Entities;

namespace Ordering.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationTicketTypeSnapshot : IEntityTypeConfiguration<TicketTypeSnapshot>
    {
        public void Configure(EntityTypeBuilder<TicketTypeSnapshot> builder)
        {
            builder.ToTable("TicketTypeSnapshot");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.ShowtimeSnapshotId).IsRequired();

            builder.Property(t => t.TicketTypeId).IsRequired();

            builder.Property(t => t.TicketTypeName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(t => t.Price)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(t => t.Capacity).IsRequired();
            builder.Property(t => t.IsReservingSeat).IsRequired();
        }
    }
}
