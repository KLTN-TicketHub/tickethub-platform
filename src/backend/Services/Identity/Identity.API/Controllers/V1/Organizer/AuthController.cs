using BuildingBlocks.API.Extensions;
using BuildingBlocks.Contracts.Models.Responses;
using Identity.Application.Common.DTOs.Auth;
using Identity.Application.Features.Auth.Commands.Organizers.ActivateOrganizer;
using Identity.Application.Features.Auth.Commands.Organizers.LoginOrganizer;
using Identity.Application.Features.Auth.Commands.Organizers.RegisterOrganizer;
using Identity.Application.Features.Auth.Requests;
using Identity.Common.Options;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace Identity.API.Controllers.V1.Organizer
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/organizer")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AuthController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly ISender _sender;
        private readonly AppUrls _appUrls;

        public AuthController(IOptions<AppSettings> appSettings, ISender sender, IOptions<AppUrls> appUrls)
        {
            _appSettings = appSettings.Value;
            _sender = sender;
            _appUrls = appUrls.Value;
        }

        [EnableRateLimiting(RateLimitPolicies.Login)]
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(
            [FromBody] LoginRequest loginRequest,
            CancellationToken cancellationToken = default)
        {
            var refreshTokenExpirationDays = _appSettings.JwtConfig?.RefreshTokenExpirationDays ?? 7;
            var result = await _sender.Send(new LoginOrganizerCommand(loginRequest), cancellationToken);

            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(refreshTokenExpirationDays)
            });

            return Ok(new AuthResult
            {
                Success = true,
                Message = "Đăng nhập thành công",
                AccessToken = result.AccessToken
            });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RegisterOrganizerAccountAsync(
            [FromBody] RegisterOrganizerRequest request,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new RegisterOrganizerCommand(request), cancellationToken);

            return Ok(new ApiResponse<OrganizerDto>
            {
                Success = true,
                Message = "Đăng ký tài khoản tổ chức thành công, vui lòng kiểm tra email để xác nhận tài khoản.",
                Data = result
            });
        }

        [AllowAnonymous]
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmailAsync(
            [FromQuery] ActivateOrganizerAccountRequest request,
            CancellationToken cancellationToken = default)
        {
            await _sender.Send(new ActivateOrganizerCommand(request), cancellationToken);

            return Redirect($"{_appUrls.FrontendBaseUrl}/organizer/login");
        }
    }
}
