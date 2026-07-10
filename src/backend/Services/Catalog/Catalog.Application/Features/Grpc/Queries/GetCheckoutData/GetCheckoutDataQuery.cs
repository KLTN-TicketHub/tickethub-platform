using MediatR;

namespace Catalog.Application.Features.Grpc.Queries.GetCheckoutData
{
    public class ValidatedTicketItemResult
    {
        public Guid? SeatId { get; set; }
        public Guid TicketTypeId { get; set; }
        public string TicketTypeName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? SeatName { get; set; }
        public string? RowName { get; set; }
        public int Quantity { get; set; }
    }

    public class GetCheckoutDataResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string EventTitle { get; set; } = string.Empty;
        public Guid OrganizerId { get; set; }
        public DateTime ShowtimeStartAt { get; set; }
        public DateTime ShowtimeEndAt { get; set; }
        public List<ValidatedTicketItemResult> TicketItems { get; set; } = new();

        public static GetCheckoutDataResult Fail(string message) => new GetCheckoutDataResult { IsSuccess = false, Message = message };
    }

    public class GetCheckoutDataItem
    {
        public Guid? SeatId { get; set; }
        public Guid TicketTypeId { get; set; }
        public int Quantity { get; set; }
    }

    public record GetCheckoutDataQuery(
        Guid EventId,
        Guid ShowtimeId,
        List<GetCheckoutDataItem> Items) : IRequest<GetCheckoutDataResult>;
}
