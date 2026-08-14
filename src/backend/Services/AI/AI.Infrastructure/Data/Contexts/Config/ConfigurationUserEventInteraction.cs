using AI.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AI.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationUserEventInteraction : IEntityTypeConfiguration<UserEventInteraction>
    {
        public void Configure(EntityTypeBuilder<UserEventInteraction> builder)
        {
            builder.ToTable("UserEventInteraction");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.EventId).IsRequired();

            builder.Property(x => x.InteractionType)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(x => x.Weight).IsRequired();
            builder.Property(x => x.OccurredAt).IsRequired();

            builder.HasIndex(x => new { x.UserId, x.EventId, x.InteractionType }).IsUnique();
        }
    }
}
