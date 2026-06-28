using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Infrastructure.Entities;

namespace Ordering.Infrastructure.Data.Contexts.Config
{
    public class OrderBookingStateConfiguration : IEntityTypeConfiguration<OrderBookingState>
    {
        public void Configure(EntityTypeBuilder<OrderBookingState> builder)
        {
            builder.ToTable("OrderBookingState");

            builder.HasKey(x => x.CorrelationId);

            builder.Property(x => x.CurrentState)
                   .HasMaxLength(64);

            builder.Property(x => x.PaymentLink)
                   .HasMaxLength(1000);

            builder.Property(x => x.TotalPrice)
                   .HasPrecision(18, 2);

            builder.Property(x => x.Version)
                   .IsConcurrencyToken();
        }
    }
}
