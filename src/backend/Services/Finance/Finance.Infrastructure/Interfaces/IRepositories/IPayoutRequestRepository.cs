using BuildingBlocks.Domain.DDD;
using Finance.Infrastructure.Data.Contexts;
using Finance.Infrastructure.Entities;

namespace Finance.Infrastructure.Interfaces.IRepositories
{
    public interface IPayoutRequestRepository : IBaseRepository<PayoutRequest, FinanceDbContext>
    {
        Task<(IEnumerable<PayoutRequestSummary> Items, int TotalCount)> GetPendingRequestsWithSummaryAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellation = default);
    }

    public class PayoutRequestSummary
    {
        public Guid PayoutRequestId { get; set; }
        public Guid EventId { get; set; }
        public string EventTitle { get; set; } = default!;
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = default!;
        public Guid OrganizerId { get; set; }
        public string OrganizerName { get; set; } = default!;
        public decimal GrossAmount { get; set; }
        public decimal RecommendedRate { get; set; }
        public int OrderCount { get; set; }
        public DateTime RequestedAt { get; set; }
    }
}
