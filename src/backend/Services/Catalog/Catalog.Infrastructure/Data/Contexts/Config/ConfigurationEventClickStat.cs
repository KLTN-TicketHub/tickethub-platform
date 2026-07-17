using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationEventClickStat : IEntityTypeConfiguration<EventClickStat>
    {
        public void Configure(EntityTypeBuilder<EventClickStat> builder)
        {
            builder.ToTable("EventClickStat");

            builder.HasKey(s => s.Id);

            builder.HasOne(s => s.Event)
                .WithMany()
                .HasForeignKey(s => s.EventId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.Property(s => s.StatDate)
                .IsRequired();

            builder.Property(s => s.ClickType)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(s => s.ClickCount)
                .IsRequired();

            builder.HasIndex(s => new { s.EventId, s.StatDate, s.ClickType })
                .IsUnique();
        }
    }
}
