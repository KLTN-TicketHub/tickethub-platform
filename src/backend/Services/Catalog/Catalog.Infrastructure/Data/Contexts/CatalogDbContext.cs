using BuildingBlocks.Infrastructure.Auditing;
using Catalog.Domain.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Data.Contexts
{
    public class CatalogDbContext : DbContext
    {
        public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
        {
        }

        #region DbSet Section
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Catalog.Domain.Entities.Event> Events { get; set; }
        public DbSet<EventApproval> EventApprovals { get; set; }
        public DbSet<EventCategory> EventCategories { get; set; }
        public DbSet<EventLocation> EventLocations { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<TicketType> TicketTypes { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Zone> Zones { get; set; }
        public DbSet<Row> Rows { get; set; }
        public DbSet<OrganizerSnapshot> OrganizerSnapshots { get; set; }
        public DbSet<ShowTime> ShowTimes { get; set; }
        public DbSet<EventRating> EventRatings { get; set; }
        public DbSet<EventCheckIn> EventCheckIns { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
        }
    }
}
