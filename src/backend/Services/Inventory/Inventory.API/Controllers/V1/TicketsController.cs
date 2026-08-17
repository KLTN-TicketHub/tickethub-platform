using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Contracts.Models.Pagination;
using BuildingBlocks.Contracts.Models.Responses;
using Inventory.Infrastructure.Dtos;
using Inventory.Infrastructure.Interfaces.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            return Ok(new ApiResponse<TicketInventoryStateDto>(true, "Lấy thông tin tồn kho thành công.", result));
        }

        [HttpPost("checkin/{qrToken}")]
        [Authorize(Roles = $"{Roles.Staff},{Roles.Organizer}")]
        public async Task<IActionResult> CheckInTicket([FromRoute] string qrToken)
        {
            if (string.IsNullOrWhiteSpace(qrToken))
            {
                return BadRequest(new ApiResponse(false, "Mã QR không được để trống."));
            }

            var ticket = await _ticketInventoryService.CheckInTicketAsync(qrToken);

            return Ok(new ApiResponse<object>(true, "Check-in thành công.", ticket));
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyTickets([FromQuery] GetMyTicketsRequest request)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out Guid userId))
            {
                throw new BuildingBlocks.Domain.Exceptions.UnauthorizedAccessException("Không thể xác định người dùng.");
            }

            var result = await _ticketInventoryService.GetMyTicketsAsync(userId, request.Status, request.PageNumber, request.PageSize);

            return Ok(new ApiResponse<PaginatedResult<UserTicketDto>>(true, "Lấy danh sách vé thành công.", result));
        }
    }
}
