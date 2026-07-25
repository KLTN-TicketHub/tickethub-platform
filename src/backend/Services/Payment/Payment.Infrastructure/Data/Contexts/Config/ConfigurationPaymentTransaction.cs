using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payment.Infrastructure.Entities;

namespace Payment.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationPaymentTransaction : IEntityTypeConfiguration<PaymentTransaction>
    {
        public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
        {
            builder.ToTable("PaymentTransaction");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.OrderId)
                .IsRequired();

            builder.Property(t => t.MerchantOrderNo)
                .IsRequired();

            builder.Property(t => t.Amount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(t => t.Gateway)
                .IsRequired();

            builder.Property(t => t.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(t => t.PayDate)
                .HasMaxLength(20);
        }
    }
}
