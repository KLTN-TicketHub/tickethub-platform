using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Contracts.Models.Responses;
using Inventory.Infrastructure.Dtos;
using Inventory.Infrastructure.Interfaces.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers.V1
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

        [HttpGet("showtimes/{showtimeId:guid}/types/{ticketTypeId:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTicketInventoryState([FromRoute] Guid showtimeId, [FromRoute] Guid ticketTypeId)
        {
            var result = await _ticketInventoryService.GetTicketInventoryStateAsync(showtimeId, ticketTypeId);
            if (result == null)
            {
                return NotFound(new ApiResponse(false, "Không tìm thấy cấu hình tồn kho cho loại vé này."));
            }
            return Ok(new ApiResponse<TicketInventoryStateDto>(true, "Lấy thông tin tồn kho thành công.", result));
        }
        [HttpPost("checkin/{qrToken}")]
        [Authorize(Roles = Roles.Staff)]
        public async Task<IActionResult> CheckInTicket([FromRoute] string qrToken)
        {
            if (string.IsNullOrWhiteSpace(qrToken))
            {
                return BadRequest(new ApiResponse(false, "Mã QR không được để trống."));
            }

            var (success, message, ticket) = await _ticketInventoryService.CheckInTicketAsync(qrToken);

            if (!success)
            {
                return BadRequest(new ApiResponse(false, message));
            }

            return Ok(new ApiResponse<object>(true, message, ticket));
        }
    }
}
