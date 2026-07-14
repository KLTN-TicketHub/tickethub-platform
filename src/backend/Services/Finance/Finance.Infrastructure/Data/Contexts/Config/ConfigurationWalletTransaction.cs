using Finance.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationWalletTransaction : IEntityTypeConfiguration<WalletTransaction>
    {
        public void Configure(EntityTypeBuilder<WalletTransaction> builder)
        {
            builder.HasKey(wt => wt.Id);

            builder.HasOne(wt => wt.Wallet)
                .WithMany(w => w.Transactions)
                .HasForeignKey(wt => wt.WalletId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(wt => wt.EventId)
                .IsRequired();

            builder.Property(wt => wt.CategoryId)
                .IsRequired();

            builder.Property(wt => wt.Amount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(wt => wt.Type)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(wt => wt.Description)
                .IsRequired()
                .HasMaxLength(500);
        }
    }
}
