using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationVenue : IEntityTypeConfiguration<Venue>
    {
        public void Configure(EntityTypeBuilder<Venue> builder)
        {
            builder.ToTable("Venue");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.VenueName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(v => v.VenueCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(v => v.VenueCode)
                .IsUnique();

            builder.Property(v => v.AddressLine)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(v => v.Ward)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(v => v.District)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(v => v.ProvinceCity)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(v => v.Country)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(v => v.Slug)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(v => v.Longitude)
                .IsRequired()
                .HasPrecision(18, 8);

            builder.Property(v => v.Latitude)
                .IsRequired()
                .HasPrecision(18, 8);

            builder.Property(v => v.PhoneNumber)
                .HasMaxLength(15);

            builder.Property(v => v.WebsiteUrl)
                .HasMaxLength(100);

            builder.Property(v => v.RowVersion)
                .IsRowVersion();
        }
    }
}
