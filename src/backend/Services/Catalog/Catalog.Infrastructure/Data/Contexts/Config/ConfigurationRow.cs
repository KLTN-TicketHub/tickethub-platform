using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationRow : IEntityTypeConfiguration<Row>
    {
        public void Configure(EntityTypeBuilder<Row> builder)
        {
            builder.ToTable("Row");
            builder.HasKey(r => r.Id);

            builder.HasOne(r => r.Zone)
                   .WithMany(z => z.Rows)
                   .HasForeignKey(r => r.ZoneId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();

            builder.Property(r => r.RowName)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(r => r.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(r => r.RowVersion)
                .IsRowVersion();
        }
    }
}
