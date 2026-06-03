using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationSeat : IEntityTypeConfiguration<Seat>
    {
        public void Configure(EntityTypeBuilder<Seat> builder)
        {
            builder.ToTable("Seat");

            builder.HasKey(s => s.Id);

            builder.HasOne(s => s.Zone)
                   .WithMany(z => z.Seats)
                   .HasForeignKey(s => s.ZoneId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(s => s.SeatCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.SeatName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.RowLabel)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.SvgElementId)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.X)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(s => s.Y)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(s => s.Radius)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(s => s.LayoutStatus)
                .IsRequired();
        }
    }
}
