using Inventory.API.Hubs;
using Inventory.Infrastructure.Interfaces.IServices;
using Microsoft.AspNetCore.SignalR;

namespace Inventory.API.Services
{
    public class SeatHubNotificationService : ISeatHubNotificationService
    {
        private readonly IHubContext<SeatMapHub> _hubContext;

        public SeatHubNotificationService(IHubContext<SeatMapHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifySeatStateChangedAsync(Guid showtimeId, Guid seatId, string status)
        {
            await _hubContext.Clients.Group($"showtime_{showtimeId}")
                .SendAsync("SeatStateChanged", new
                {
                    seatId = seatId.ToString(),
                    status
                });
        }
    }
}
