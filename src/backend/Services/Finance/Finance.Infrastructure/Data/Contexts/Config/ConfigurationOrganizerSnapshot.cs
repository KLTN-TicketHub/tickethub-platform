using Finance.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationOrganizerSnapshot : IEntityTypeConfiguration<OrganizerSnapshot>
    {
        public void Configure(EntityTypeBuilder<OrganizerSnapshot> builder)
        {
            builder.ToTable("OrganizerSnapshot");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.OrganizerName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(o => o.ImageUrl)
                .HasMaxLength(500);

            builder.Property(o => o.Email)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(o => o.PhoneNumber)
                .HasMaxLength(20);
        }
    }
}
