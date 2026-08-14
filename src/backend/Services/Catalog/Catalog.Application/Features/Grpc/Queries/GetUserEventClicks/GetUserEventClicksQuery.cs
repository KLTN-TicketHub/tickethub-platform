using MediatR;

namespace Catalog.Application.Features.Grpc.Queries.GetUserEventClicks
{
    public class UserEventClickItemResult
    {
        public Guid UserId { get; set; }
        public Guid EventId { get; set; }
        public string ClickType { get; set; } = string.Empty;
        public DateTime ClickedAt { get; set; }
    }

    public class GetUserEventClicksResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<UserEventClickItemResult> Clicks { get; set; } = new();

        public static GetUserEventClicksResult Fail(string message) => new GetUserEventClicksResult { IsSuccess = false, Message = message };
    }

    public record GetUserEventClicksQuery(DateOnly From, DateOnly To) : IRequest<GetUserEventClicksResult>;
}
