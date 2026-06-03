using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationZonePricing : IEntityTypeConfiguration<ZonePricing>
    {
        public void Configure(EntityTypeBuilder<ZonePricing> builder)
        {
            builder.ToTable("ZonePricing");

            builder.HasKey(zp => zp.Id);

            builder.HasOne(zp => zp.Event)
                .WithMany()
                .HasForeignKey(zp => zp.EventId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.HasOne(zp => zp.Zone)
                .WithMany()
                .HasForeignKey(zp => zp.ZoneId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.HasOne(zp => zp.TicketType)
                .WithMany()
                .HasForeignKey(zp => zp.TicketTypeId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.Property(zp => zp.ListedPrice)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(zp => zp.PublishedQuota)
                .IsRequired();

            builder.Property(zp => zp.Status)
                .IsRequired()
                .HasConversion<string>();
        }
    }
}