using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationTicketType : IEntityTypeConfiguration<TicketType>
    {
        public void Configure(EntityTypeBuilder<TicketType> builder)
        {
            builder.ToTable("TicketType");

            builder.HasKey(tt => tt.Id);

            builder.HasOne(tt => tt.Event)
                .WithMany(e => e.TicketTypes)
                .HasForeignKey(tt => tt.EventId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.HasOne(tt => tt.Zone)
                .WithMany()
                .HasForeignKey(tt => tt.ZoneId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(tt => tt.TicketTypeName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(tt => tt.Description)
                .HasMaxLength(500);

            builder.Property(tt => tt.Price)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(tt => tt.PublishedQuota)
                .IsRequired();

            builder.Property(tt => tt.MinQtyQuota)
                .IsRequired();

            builder.Property(tt => tt.MaxQtyQuota)
                .IsRequired();

            builder.Property(tt => tt.DisplayOrder)
                .IsRequired();

            builder.Property(tt => tt.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(tt => tt.RowVersion)
                .IsRowVersion();
        }
    }
}