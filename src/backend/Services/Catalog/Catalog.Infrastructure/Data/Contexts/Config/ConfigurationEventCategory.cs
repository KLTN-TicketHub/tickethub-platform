using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationEventCategory : IEntityTypeConfiguration<EventCategory>
    {
        public void Configure(EntityTypeBuilder<EventCategory> builder)
        {
            builder.ToTable("EventCategory");

            builder.HasKey(ec => ec.Id);

            builder.Property(ec => ec.CategoryCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(ec => ec.CategoryName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(ec => ec.Slug)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(ec => ec.Description)
                .HasMaxLength(500);

            builder.Property(ec => ec.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(ec => ec.RowVersion)
                .IsRowVersion();

            builder.HasMany(ec => ec.Events)
                .WithMany(e => e.Categories);
        }
    }
}
