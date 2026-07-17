using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationUserEventClick : IEntityTypeConfiguration<UserEventClick>
    {
        public void Configure(EntityTypeBuilder<UserEventClick> builder)
        {
            builder.ToTable("UserEventClick");

            builder.HasKey(c => c.Id);

            builder.HasOne(c => c.Event)
                .WithMany()
                .HasForeignKey(c => c.EventId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.Property(c => c.UserId)
                .IsRequired();

            builder.Property(c => c.ClickType)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(c => c.ClickedAt)
                .IsRequired();

            builder.HasIndex(c => new { c.EventId, c.UserId });
        }
    }
}
