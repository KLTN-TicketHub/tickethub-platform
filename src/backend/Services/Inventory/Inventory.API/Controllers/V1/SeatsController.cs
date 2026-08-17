using BuildingBlocks.Application.Interfaces;
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
    public class SeatsController : ControllerBase
    {
        private readonly ISeatStateService _seatStateService;
        private readonly ICurrentUserService _currentUserService;

        public SeatsController(
            ISeatStateService seatStateService,
            ICurrentUserService currentUserService)
        {
            _seatStateService = seatStateService;
            _currentUserService = currentUserService;
        }

        [HttpGet("showtimes/{showtimeId}/seats")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSeatStates(Guid showtimeId)
        {
            var result = await _seatStateService.GetSeatStatesAsync(showtimeId);
            return Ok(new ApiResponse<IEnumerable<SeatStateDto>>(true, "Lấy trạng thái ghế thành công.", result));
        }

        [HttpPost("seats/lock")]
        public async Task<IActionResult> LockSeat([FromBody] LockSeatRequest request)
        {
            Guid userId = _currentUserService.UserId
                ?? throw new BuildingBlocks.Domain.Exceptions.UnauthorizedAccessException("Bạn cần đăng nhập để thực hiện chức năng này.");

            await _seatStateService.LockSeatAsync(request.ShowtimeId, request.SeatId, userId);

            return Ok(new ApiResponse(true, "Khóa ghế thành công."));
        }

        [HttpPost("seats/unlock")]
        public async Task<IActionResult> UnlockSeat([FromBody] LockSeatRequest request)
        {
            Guid userId = _currentUserService.UserId
                ?? throw new BuildingBlocks.Domain.Exceptions.UnauthorizedAccessException("Bạn cần đăng nhập để thực hiện chức năng này.");

            await _seatStateService.UnlockSeatAsync(request.ShowtimeId, request.SeatId, userId);

            return Ok(new ApiResponse(true, "Hủy khóa ghế thành công."));
        }
    }

    public class LockSeatRequest
    {
        public Guid ShowtimeId { get; set; }
        public Guid SeatId { get; set; }
    }
}
