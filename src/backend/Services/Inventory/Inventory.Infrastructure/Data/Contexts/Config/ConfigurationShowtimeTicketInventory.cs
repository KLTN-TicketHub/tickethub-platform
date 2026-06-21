using Inventory.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationShowtimeTicketInventory : IEntityTypeConfiguration<ShowtimeTicketInventory>
    {
        public void Configure(EntityTypeBuilder<ShowtimeTicketInventory> builder)
        {
            builder.ToTable("ShowtimeTicketInventory");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.ShowTimeId)
                .IsRequired();

            builder.Property(s => s.TicketTypeId)
                .IsRequired();

            builder.Property(s => s.Capacity)
                .IsRequired();

            builder.Property(s => s.SoldQuantity)
                .IsRequired();

            builder.Property(s => s.ReservedQuantity)
                .IsRequired();

            builder.Property(s => s.RowVersion)
                .IsRowVersion();
        }
    }
}
