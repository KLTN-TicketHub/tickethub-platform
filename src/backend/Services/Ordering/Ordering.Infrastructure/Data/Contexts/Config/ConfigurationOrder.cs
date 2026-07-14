using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Infrastructure.Entities;

namespace Ordering.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationOrder : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Order");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.UserId)
                .IsRequired();

            builder.Property(o => o.CustomerName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(o => o.CustomerEmail)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(o => o.CustomerPhone)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(o => o.ShowTimeId)
                .IsRequired();

            builder.Property(o => o.EventId)
                .IsRequired();

            builder.Property(o => o.EventTitle)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(o => o.CategoryId)
                .IsRequired();

            builder.Property(o => o.CategoryName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(o => o.ShowtimeStartAt)
                .IsRequired();

            builder.Property(o => o.TotalPrice)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(o => o.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(o => o.PaymentMethod)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(o => o.RowVersion)
                .IsRowVersion();

            builder.HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
