namespace Catalog.Application.Features.Events.Requests
{
    public class ReviewEventRequest
    {
        public bool IsApproved { get; set; }
        public string? Reason { get; set; }
    }
}
