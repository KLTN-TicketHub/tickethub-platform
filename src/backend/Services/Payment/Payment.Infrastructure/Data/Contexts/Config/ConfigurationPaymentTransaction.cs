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


        }
    }
}
