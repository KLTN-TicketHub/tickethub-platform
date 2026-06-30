namespace Inventory.Infrastructure.Interfaces.IServices
{
    public interface ICatalogService
    {
        Task<(bool IsSuccess, string Message)> ValidateSeatLockAsync(
            Guid showtimeId,
            List<Guid> seatIds);

        Task<(bool IsSuccess, string Message)> ValidateTicketTypesAsync(
            Guid showtimeId,
            List<(Guid TicketTypeId, int Quantity)> ticketItems);
    }
}
