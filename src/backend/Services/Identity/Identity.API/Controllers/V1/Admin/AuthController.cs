using BuildingBlocks.API.Extensions;
using BuildingBlocks.Contracts.Models.Responses;
using Identity.Application.Features.Auth.Commands.LoginAdmin.Confirm;
using Identity.Application.Features.Auth.Commands.LoginAdmin.Initiate;
using Identity.Application.Features.Auth.Request;
using Identity.Common.Options;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace Identity.API.Controllers.V1.Admin
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AuthController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly ISender _sender;

        public AuthController(IOptions<AppSettings> appSettings, ISender sender)
        {
            _appSettings = appSettings.Value;
            _sender = sender;
        }

        [EnableRateLimiting(RateLimitPolicies.Login)]
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(
            [FromBody] LoginRequest loginRequest,
            CancellationToken cancellationToken = default)
        {
            await _sender.Send(new InitiateAdminLoginCommand(loginRequest), cancellationToken);

            return Ok(new
            {
                Success = true,
                Message = "Mã xác thực đã được gửi tới email của bạn"
            });
        }

        [EnableRateLimiting(RateLimitPolicies.Login)]
        [AllowAnonymous]
        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmAsync(
            [FromBody] ConfirmLoginRequest confirmRequest,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new ConfirmAdminLoginCommand(confirmRequest), cancellationToken);

            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(_appSettings.JwtConfig!.RefreshTokenExpirationDays)
            });

            return Ok(new AuthResult
            {
                Success = true,
                Message = "Đăng nhập thành công",
                AccessToken = result.AccessToken
            });
        }
    }
}
