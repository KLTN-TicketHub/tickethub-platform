using BuildingBlocks.API.Extensions;
using BuildingBlocks.Contracts.Models.Responses;
using Identity.Application.Common.DTOs.Admin;
using Identity.Application.Features.Admin.Moderators.Commands.RegisterModerator;
using Identity.Application.Features.Admin.Moderators.Requests;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Identity.API.Controllers.V1.Admin
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class ModeratorsController : ControllerBase
    {
        private readonly ISender _sender;

        public ModeratorsController(ISender sender)
        {
            _sender = sender;
        }

        [EnableRateLimiting(RateLimitPolicies.PerUser)]
        [HttpPost("register")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> RegisterModeratorAsync(
            [FromForm] RegisterModeratorRequest request,
            CancellationToken cancellationToken = default)
        {
            ModeratorDto result = await _sender.Send(new RegisterModeratorCommand(request), cancellationToken);

            return Ok(new ApiResponse<ModeratorDto>
            {
                Success = true,
                Message = "Tạo tài khoản Moderator thành công. Thông tin đăng nhập đã được gửi qua email.",
                Data = result
            });
        }
    }
}
