using Inventory.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationIssuedTicket : IEntityTypeConfiguration<IssuedTicket>
    {
        public void Configure(EntityTypeBuilder<IssuedTicket> builder)
        {
            builder.ToTable("IssuedTicket");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.OrderId).IsRequired();
            builder.Property(t => t.ShowTimeId).IsRequired();
            builder.Property(t => t.EventId).IsRequired();
            builder.Property(t => t.UserId).IsRequired();

            builder.Property(t => t.EventTitle)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(t => t.CustomerName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.CustomerEmail)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.SeatName).HasMaxLength(50);
            builder.Property(t => t.RowName).HasMaxLength(50);

            builder.Property(t => t.TicketTypeId).IsRequired();
            builder.Property(t => t.TicketTypeName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Price)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(t => t.QrCodeToken)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(t => t.QrCodeToken).IsUnique();

            builder.Property(t => t.QrCodeBase64)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            builder.Property(t => t.ShowtimeStartAt).IsRequired();

            builder.Property(t => t.Status)
                .IsRequired()
                .HasConversion<string>();
        }
    }
}
