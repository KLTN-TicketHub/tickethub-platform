using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Inventory.API.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SeatMapHub : Hub
    {
        public async Task JoinShowtime(string showtimeId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"showtime_{showtimeId}");
        }

        public async Task LeaveShowtime(string showtimeId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"showtime_{showtimeId}");
        }
    }
}
