using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationZone : IEntityTypeConfiguration<Zone>
    {
        public void Configure(EntityTypeBuilder<Zone> builder)
        {
            builder.ToTable("Zone");

            builder.HasKey(z => z.Id);

            builder.HasOne(z => z.SeatMap)
                .WithMany(s => s.Zones)
                .HasForeignKey(z => z.SeatMapId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.Property(z => z.ZoneName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(z => z.ZoneCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(z => z.ZoneType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(z => z.Color)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(z => z.X)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(z => z.Y)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(z => z.Width)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(z => z.Height)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(z => z.IsStage)
                .IsRequired();

            builder.Property(z => z.IsReservingSeat)
                .IsRequired();

            builder.Property(z => z.IsSalable)
                .IsRequired();

            builder.Property(z => z.SvgElementId)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(z => z.ElementJson);

            builder.Property(z => z.Capacity)
                .IsRequired();

            builder.Property(z => z.DisplayOrder)
                .IsRequired();

            builder.Property(z => z.BasePrice)
                .HasPrecision(18, 2);

            builder.Property(z => z.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(z => z.RowVersion)
                .IsRowVersion();
        }
    }
}
