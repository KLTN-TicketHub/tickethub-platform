namespace Ordering.Infrastructure.Interfaces.IServices
{
    public interface ICatalogService
    {
        Task<(bool IsSuccess, string Message)> ValidateCheckoutAsync(
            Guid eventId,
            Guid showtimeId,
            List<Guid> seatIds,
            List<CheckoutTicketValidationItem> ticketItems);
    }

    public class CheckoutTicketValidationItem
    {
        public Guid TicketTypeId { get; set; }
        public int Quantity { get; set; }
    }
}
