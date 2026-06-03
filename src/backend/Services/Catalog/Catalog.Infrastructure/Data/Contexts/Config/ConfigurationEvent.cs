using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationEvent : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.ToTable("Event");

            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.Venue)
                .WithMany()
                .HasForeignKey(e => e.VenueId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.SeatMap)
                .WithMany()
                .HasForeignKey(e => e.SeatMapId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Slug)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(5000);

            builder.Property(e => e.StartAt)
                .IsRequired();

            builder.Property(e => e.EndAt)
                .IsRequired();

            builder.Property(e => e.SaleOpenAt)
                .IsRequired();

            builder.Property(e => e.SaleCloseAt)
                .IsRequired();

            builder.Property(e => e.CurrencyCode)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(e => e.CoverImageUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(e => e.CustomVenueName)
                .HasMaxLength(200);

            builder.Property(e => e.CustomAddressLine)
                .HasMaxLength(200);

            builder.Property(e => e.CustomWard)
                .HasMaxLength(100);
            
            builder.Property(e => e.CustomDistrict)
                .HasMaxLength(100);

            builder.Property(e => e.CustomProvinceCity)
                .HasMaxLength(100);

            builder.Property(e => e.CustomCountry)
                .HasMaxLength(100);

            builder.Property(e => e.RowVersion)
                .IsRowVersion();

            builder.HasMany(e => e.Categories)
                .WithMany(ec => ec.Events);
        }
    }
}
