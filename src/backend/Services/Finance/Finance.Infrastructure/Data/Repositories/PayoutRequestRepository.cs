using BuildingBlocks.Infrastructure.Data;
using Finance.Infrastructure.Data.Contexts;
using Finance.Infrastructure.Entities;
using Finance.Infrastructure.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Finance.Infrastructure.Data.Repositories
{
    public class PayoutRequestRepository : BaseRepository<PayoutRequest, FinanceDbContext>, IPayoutRequestRepository
    {
        public PayoutRequestRepository(FinanceDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<(IEnumerable<PayoutRequestSummary> Items, int TotalCount)> GetPendingRequestsWithSummaryAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellation = default)
        {
            var pendingRevenueByEvent = _dbContext.Set<WalletTransaction>()
                .Where(t => t.Status == WalletTransactionStatus.Pending
                         && t.Type == WalletTransactionType.Revenue
                         && t.EventPayoutId == null)
                .GroupBy(t => t.EventId)
                .Select(g => new { EventId = g.Key, GrossAmount = g.Sum(t => t.Amount), OrderCount = g.Count() });

            IQueryable<PayoutRequestSummary> query =
                from pr in _dbContext.Set<PayoutRequest>()
                where pr.Status == PayoutRequestStatus.Pending
                join tx in pendingRevenueByEvent on pr.EventId equals tx.EventId
                join cs in _dbContext.Set<CommissionSetting>() on pr.CategoryId equals cs.CategoryId into csJoin
                from cs in csJoin.DefaultIfEmpty()
                join os in _dbContext.Set<OrganizerSnapshot>() on pr.OrganizerId equals os.Id into osJoin
                from os in osJoin.DefaultIfEmpty()
                let lastRejection = _dbContext.Set<EventPayout>()
                    .Where(p => p.EventId == pr.EventId && p.Status == EventPayoutStatus.Rejected)
                    .OrderByDescending(p => p.RejectedAt)
                    .FirstOrDefault()
                select new PayoutRequestSummary
                {
                    PayoutRequestId = pr.Id,
                    EventId = pr.EventId,
                    EventTitle = pr.EventTitle,
                    CategoryId = pr.CategoryId,
                    CategoryName = cs != null ? cs.CategoryName : string.Empty,
                    OrganizerId = pr.OrganizerId,
                    OrganizerName = os != null ? os.OrganizerName : string.Empty,
                    GrossAmount = tx.GrossAmount,
                    RecommendedRate = cs != null ? cs.Rate : 0,
                    OrderCount = tx.OrderCount,
                    RequestedAt = pr.CreatedAt,
                    IsResubmitted = lastRejection != null,
                    LastRejectionReason = lastRejection != null ? lastRejection.RejectionReason : null,
                    LastRejectedAt = lastRejection != null ? lastRejection.RejectedAt : null
                };

            int totalCount = await query.CountAsync(cancellation);

            List<PayoutRequestSummary> items = await query
                .OrderBy(s => s.RequestedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellation);

            return (items, totalCount);
        }
    }
}
