using Inventory.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationShowtimeSeat : IEntityTypeConfiguration<ShowtimeSeat>
    {
        public void Configure(EntityTypeBuilder<ShowtimeSeat> builder)
        {
            builder.ToTable("ShowtimeSeat");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.ShowTimeId)
                .IsRequired();

            builder.Property(s => s.SeatId)
                .IsRequired();

            builder.Property(s => s.UserId)
                .IsRequired();

            builder.Property(s => s.Price)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(s => s.Row)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.SeatName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.SeatStatus)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(s => s.IsCheckedIn)
                .IsRequired();
        }
    }
}
