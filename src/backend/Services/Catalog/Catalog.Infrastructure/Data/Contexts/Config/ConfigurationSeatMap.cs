using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationSeatMap : IEntityTypeConfiguration<SeatMap>
    {
        public void Configure(EntityTypeBuilder<SeatMap> builder)
        {
            builder.ToTable("SeatMap");

            builder.HasKey(s => s.Id);

            builder.HasOne(s => s.Venue)
                .WithMany(v => v.SeatMaps)
                .HasForeignKey(s => s.VenueId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.Property(s => s.SeatMapName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.SeatMapCode)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.SvgFileUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(s => s.Width)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(s => s.Height)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(s => s.RowVersion)
                .IsRowVersion();
        }
    }
}
