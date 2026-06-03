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

            builder.Property(tt => tt.TicketTypeName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(tt => tt.TicketTypeCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(tt => tt.Description)
                .HasMaxLength(500);

            builder.Property(tt => tt.Color)
                .IsRequired()
                .HasMaxLength(20);

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