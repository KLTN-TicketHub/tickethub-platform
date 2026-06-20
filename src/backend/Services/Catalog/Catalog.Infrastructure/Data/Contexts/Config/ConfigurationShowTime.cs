using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationShowTime : IEntityTypeConfiguration<ShowTime>
    {
        public void Configure(EntityTypeBuilder<ShowTime> builder)
        {
            builder.ToTable("ShowTime");

            builder.HasKey(st => st.Id);

            builder.HasOne(st => st.Event)
                .WithMany(e => e.ShowTimes)
                .HasForeignKey(st => st.EventId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.Property(st => st.StartAt)
                .IsRequired();

            builder.Property(st => st.EndAt)
                .IsRequired();

            builder.Property(st => st.Status)
                .IsRequired();

            builder.Property(st => st.RowVersion)
                .IsRowVersion();
        }
}
}
