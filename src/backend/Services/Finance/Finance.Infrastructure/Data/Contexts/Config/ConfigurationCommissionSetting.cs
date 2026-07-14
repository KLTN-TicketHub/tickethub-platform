using Finance.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationCommissionSetting : IEntityTypeConfiguration<CommissionSetting>
    {
        public void Configure(EntityTypeBuilder<CommissionSetting> builder)
        {
            builder.ToTable("CommissionSetting");

            builder.HasKey(cs => cs.Id);

            builder.HasIndex(cs => cs.CategoryId).IsUnique();

            builder.Property(cs => cs.CategoryId)
                .IsRequired();

            builder.Property(cs => cs.CategoryName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(cs => cs.Rate)
                .IsRequired()
                .HasPrecision(5, 2);
        }
    }
}
