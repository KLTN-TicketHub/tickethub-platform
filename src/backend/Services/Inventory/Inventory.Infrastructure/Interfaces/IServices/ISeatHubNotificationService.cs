namespace Inventory.Infrastructure.Interfaces.IServices
{
    public interface ISeatHubNotificationService
    {
        Task NotifySeatStateChangedAsync(Guid showtimeId, Guid seatId, string status);
    }
}
