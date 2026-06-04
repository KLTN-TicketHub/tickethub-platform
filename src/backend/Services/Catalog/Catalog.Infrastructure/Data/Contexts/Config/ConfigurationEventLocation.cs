using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationEventLocation : IEntityTypeConfiguration<EventLocation>
    {
        public void Configure(EntityTypeBuilder<EventLocation> builder)
        {
            builder.ToTable("EventLocation");

            builder.HasKey(el => el.Id);

            builder.Property(el => el.VenueName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(el => el.AddressLine)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(el => el.Ward)
                .HasMaxLength(100);

            builder.Property(el => el.District)
                .HasMaxLength(100);

            builder.Property(el => el.ProvinceCity)
                .HasMaxLength(100);

            builder.Property(el => el.Country)
                .HasMaxLength(100);

            builder.HasOne(el => el.Event)
                .WithOne(e => e.Location)
                .HasForeignKey<EventLocation>(el => el.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
