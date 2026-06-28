using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Infrastructure.Entities;

namespace Ordering.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationOrderItem : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItem");

            builder.Property(oi => oi.OrderId)
                .IsRequired();

            builder.Property(oi => oi.SeatName)
                .HasMaxLength(100);

            builder.Property(oi => oi.RowName)
                .HasMaxLength(100);

            builder.Property(oi => oi.TicketTypeId)
                .IsRequired();

            builder.Property(oi => oi.TicketTypeName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(oi => oi.Quantity)
                .IsRequired();

            builder.Property(oi => oi.Price)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
