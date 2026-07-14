using Finance.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationPayoutRequest : IEntityTypeConfiguration<PayoutRequest>
    {
        public void Configure(EntityTypeBuilder<PayoutRequest> builder)
        {
            builder.ToTable("PayoutRequest");

            builder.HasKey(pr => pr.Id);

            builder.HasIndex(pr => new { pr.EventId, pr.Status });

            builder.Property(pr => pr.EventId)
                .IsRequired();

            builder.Property(pr => pr.EventTitle)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(pr => pr.CategoryId)
                .IsRequired();

            builder.Property(pr => pr.OrganizerId)
                .IsRequired();

            builder.Property(pr => pr.WalletId)
                .IsRequired();

            builder.Property(pr => pr.Status)
                .IsRequired()
                .HasConversion<string>();
        }
    }
}
