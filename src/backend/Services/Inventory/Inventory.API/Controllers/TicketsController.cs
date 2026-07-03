using BuildingBlocks.Contracts.Models.Responses;
using Inventory.Infrastructure.Dtos;
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

        public TicketsController(ITicketInventoryService ticketInventoryService)
        {
            _ticketInventoryService = ticketInventoryService;
        }

        [HttpGet("showtimes/{showtimeId}/types/{ticketTypeId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTicketInventoryState(Guid showtimeId, Guid ticketTypeId)
        {
            var result = await _ticketInventoryService.GetTicketInventoryStateAsync(showtimeId, ticketTypeId);
            if (result == null)
            {
                return NotFound(new ApiResponse(false, "Không tìm thấy cấu hình tồn kho cho loại vé này."));
            }
            return Ok(new ApiResponse<TicketInventoryStateDto>(true, "Lấy thông tin tồn kho thành công.", result));
        }
    }
}
