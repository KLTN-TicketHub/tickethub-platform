using Finance.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationWallet : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.HasKey(w => w.Id);

            builder.Property(w => w.OrganizerId)
                .IsRequired();

            builder.Property(w => w.Balance)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(w => w.RowVersion)
                .IsRowVersion();
        }
    }
}
