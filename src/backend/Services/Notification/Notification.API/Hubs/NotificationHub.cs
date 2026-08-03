using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Notification.API.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            string? userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            }

            IEnumerable<Claim> roleClaims = Context.User?.FindAll(ClaimTypes.Role) ?? Enumerable.Empty<Claim>();

            foreach (Claim roleClaim in roleClaims)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"role_{roleClaim.Value}");
            }

            await base.OnConnectedAsync();
        }
    }
}
