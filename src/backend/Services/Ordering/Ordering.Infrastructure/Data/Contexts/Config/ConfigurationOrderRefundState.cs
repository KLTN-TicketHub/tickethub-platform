using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Infrastructure.Entities;

namespace Ordering.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationOrderRefundState : IEntityTypeConfiguration<OrderRefundState>
    {
        public void Configure(EntityTypeBuilder<OrderRefundState> builder)
        {
            builder.ToTable("OrderRefundState");

            builder.HasKey(x => x.CorrelationId);

            builder.Property(x => x.CurrentState)
                   .HasMaxLength(64);

            builder.Property(x => x.RefundableAmount)
                   .HasPrecision(18, 2);

            builder.Property(x => x.FailureReason)
                   .HasMaxLength(500);

            builder.Property(x => x.Version)
                   .IsConcurrencyToken();
        }
    }
}
