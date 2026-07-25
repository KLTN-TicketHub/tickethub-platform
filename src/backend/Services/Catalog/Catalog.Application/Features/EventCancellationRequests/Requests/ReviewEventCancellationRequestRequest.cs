namespace Catalog.Application.Features.EventCancellationRequests.Requests
{
    public class ReviewEventCancellationRequestRequest
    {
        public bool IsApproved { get; set; }
        public string? Reason { get; set; }
    }
}
