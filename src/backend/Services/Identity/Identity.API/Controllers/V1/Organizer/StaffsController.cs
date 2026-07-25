using BuildingBlocks.API.Extensions;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Contracts.Models.Responses;
using Identity.Application.Common.DTOs.Organizer;
using Identity.Application.Features.Organizer.Staffs.Commands.RegisterStaff;
using Identity.Application.Features.Organizer.Staffs.Requests;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Identity.API.Controllers.V1.Organizer
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/organizer/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Roles.Organizer)]
    public class StaffsController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly ICurrentUserService _currentUserService;

        public StaffsController(ISender sender, ICurrentUserService currentUserService)
        {
            _sender = sender;
            _currentUserService = currentUserService;
        }

        [EnableRateLimiting(RateLimitPolicies.PerUser)]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterStaffAsync(
            [FromBody] RegisterStaffRequest request,
            CancellationToken cancellationToken = default)
        {
            Guid organizerId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("Không thể xác định danh tính người tổ chức.");

            StaffDto result = await _sender.Send(new RegisterStaffCommand(organizerId, request), cancellationToken);

            return Ok(new ApiResponse<StaffDto>
            {
                Success = true,
                Message = "Tạo tài khoản nhân viên soát vé thành công. Thông tin đăng nhập đã được gửi qua email.",
                Data = result
            });
        }
    }
}
