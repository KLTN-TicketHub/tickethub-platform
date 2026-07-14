using Finance.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationEventPayout : IEntityTypeConfiguration<EventPayout>
    {
        public void Configure(EntityTypeBuilder<EventPayout> builder)
        {
            builder.ToTable("EventPayout");

            builder.HasKey(ep => ep.Id);

            builder.Property(ep => ep.EventId)
                .IsRequired();

            builder.Property(ep => ep.EventTitle)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(ep => ep.CategoryId)
                .IsRequired();

            builder.Property(ep => ep.OrganizerId)
                .IsRequired();

            builder.Property(ep => ep.WalletId)
                .IsRequired();

            builder.Property(ep => ep.GrossAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(ep => ep.RecommendedRate)
                .IsRequired()
                .HasPrecision(5, 2);

            builder.Property(ep => ep.AppliedRate)
                .IsRequired()
                .HasPrecision(5, 2);

            builder.Property(ep => ep.FeeAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(ep => ep.NetAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(ep => ep.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(ep => ep.ReviewedByUserId)
                .IsRequired();

            builder.Property(ep => ep.ReviewedByName)
                .HasMaxLength(200);

            builder.Property(ep => ep.ReviewedAt)
                .IsRequired();
        }
    }
}
