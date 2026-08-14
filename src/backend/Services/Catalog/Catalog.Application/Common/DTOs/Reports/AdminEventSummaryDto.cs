namespace Catalog.Application.Common.DTOs.Reports
{
    public class AdminEventSummaryDto
    {
        public int TotalEvents { get; set; }

        public int PendingApprovalCount { get; set; }

        public int PublishedCount { get; set; }

        public int RejectedCount { get; set; }

        public int CancelledCount { get; set; }

        public int ArchivedCount { get; set; }
    }
}
