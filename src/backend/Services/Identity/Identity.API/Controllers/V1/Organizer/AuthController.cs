using BuildingBlocks.API.Extensions;
using BuildingBlocks.Contracts.Models.Responses;
using Identity.Application.Features.Auth.Commands.LoginOrganizer;
using Identity.Application.Features.Auth.Request;
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
    [Route("api/v{version:apiVersion}/organizer/[controller]")]
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
            var refreshTokenExpirationDays = _appSettings.JwtConfig?.RefreshTokenExpirationDays ?? 7;
            var result = await _sender.Send(new LoginOrganizerCommand(loginRequest), cancellationToken);

            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(refreshTokenExpirationDays)
            });

            return Ok(new AuthResult
            {
                Success = true,
                Message = "Login successful",
                AccessToken = result.AccessToken
            });
        }
    }
}
