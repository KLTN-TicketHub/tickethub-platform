using BuildingBlocks.Application.Interfaces;
using Inventory.Infrastructure.Interfaces.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketInventoryService _ticketInventoryService;
        private readonly ICurrentUserService _currentUserService;

        public TicketsController(
            ITicketInventoryService ticketInventoryService,
            ICurrentUserService currentUserService)
        {
            _ticketInventoryService = ticketInventoryService;
            _currentUserService = currentUserService;
        }

        [HttpGet("showtimes/{showtimeId}/types/{ticketTypeId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTicketInventoryState(Guid showtimeId, Guid ticketTypeId)
        {
            var result = await _ticketInventoryService.GetTicketInventoryStateAsync(showtimeId, ticketTypeId);
            if (result == null)
            {
                return NotFound(new { message = "Không tìm thấy cấu hình tồn kho cho loại vé này." });
            }
            return Ok(result);
        }
    }
}
