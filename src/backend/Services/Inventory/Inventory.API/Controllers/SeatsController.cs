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
            return Ok(result);
        }

        [HttpPost("seats/lock")]
        public async Task<IActionResult> LockSeat([FromBody] LockSeatRequest request)
        {
            if (_currentUserService.UserId == null)
            {
                return Unauthorized(new { message = "Bạn cần đăng nhập để thực hiện chức năng này." });
            }

            var success = await _seatStateService.LockSeatAsync(request.ShowtimeId, request.SeatId, _currentUserService.UserId.Value);
            if (!success)
            {
                return BadRequest(new { message = "Ghế đang được chọn bởi người khác hoặc đã bán." });
            }

            return Ok(new { success = true, message = "Khóa ghế thành công." });
        }

        [HttpPost("seats/unlock")]
        public async Task<IActionResult> UnlockSeat([FromBody] LockSeatRequest request)
        {
            if (_currentUserService.UserId == null)
            {
                return Unauthorized(new { message = "Bạn cần đăng nhập để thực hiện chức năng này." });
            }

            var success = await _seatStateService.UnlockSeatAsync(request.ShowtimeId, request.SeatId, _currentUserService.UserId.Value);
            if (!success)
            {
                return BadRequest(new { message = "Bạn không thể hủy khóa ghế của người khác." });
            }

            return Ok(new { success = true, message = "Hủy khóa ghế thành công." });
        }
    }

    public class LockSeatRequest
    {
        public Guid ShowtimeId { get; set; }
        public Guid SeatId { get; set; }
    }
}
