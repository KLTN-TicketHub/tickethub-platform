using Catalog.Domain.Entities;
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

            builder.HasOne(e => e.SeatMap)
                .WithMany()
                .HasForeignKey(e => e.SeatMapId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Organizer)
                .WithMany(e => e.Events)
                .HasForeignKey(e => e.OrganizerId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.HasOne(e => e.Category)
                .WithMany(e => e.Events)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();


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

            builder.Property(e => e.RowVersion)
                .IsRowVersion();

            builder.HasOne(e => e.Location)
                .WithOne(e => e.Event)
                .HasForeignKey<EventLocation>(el => el.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.ShowTimes)
                .WithOne(st => st.Event)
                .HasForeignKey(st => st.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Approval)
               .WithOne(a => a.Event)
               .HasForeignKey<EventApproval>(ea => ea.EventId)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
