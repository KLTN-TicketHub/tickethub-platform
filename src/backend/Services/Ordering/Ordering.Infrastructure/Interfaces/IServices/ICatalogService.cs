namespace Ordering.Infrastructure.Interfaces.IServices
{
    public interface ICatalogService
    {
        Task<CheckoutDataResult> GetCheckoutDataAsync(
            Guid eventId,
            Guid showtimeId,
            List<Ordering.Common.Dtos.CheckoutItemDto> items);
    }

    public class CheckoutDataResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<ValidatedCheckoutTicketItemDto> TicketItems { get; set; } = new();
    }

    public class ValidatedCheckoutTicketItemDto
    {
        public Guid? SeatId { get; set; }
        public Guid TicketTypeId { get; set; }
        public string TicketTypeName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string SeatName { get; set; } = string.Empty;
        public string RowName { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
