namespace Ordering.Infrastructure.Interfaces.IServices
{
    public interface IInventoryService
    {
        Task<(bool IsSuccess, string Message)> UpgradeSeatLocksAsync(Guid showtimeId, List<Guid> seatIds, Guid userId);

        Task<(bool IsSuccess, string Message)> LockTicketQuantitiesAsync(Guid showtimeId, List<(Guid TicketTypeId, int Quantity)> items, Guid userId);
    }
}
