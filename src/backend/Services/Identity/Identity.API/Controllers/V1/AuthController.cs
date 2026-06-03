using BuildingBlocks.API.Extensions;
using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Contracts.Models.Responses;
using Identity.Application.Common.DTOs.Auth;
using Identity.Application.Features.Auth.Commands.LoginGoogle;
using Identity.Application.Features.Auth.Commands.Logout;
using Identity.Application.Features.Auth.Commands.Refresh;
using Identity.Application.Features.Auth.Queries.GetProfile;
using Identity.Application.Features.Auth.Requests;
using Identity.Common.Options;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace Identity.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AuthController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly ISender _sender;
        private readonly IMemoryCache _memoryCache;
        private readonly GoogleAuthSettings _googleAuthSettings;

        public AuthController(
            IOptions<AppSettings> appSettings,
            ISender sender,
            IMemoryCache memoryCache,
            IOptions<GoogleAuthSettings> googleAuthSettings)
        {
            _appSettings = appSettings.Value;
            _sender = sender;
            _memoryCache = memoryCache;
            _googleAuthSettings = googleAuthSettings.Value;
        }

        [HttpGet("profile")]
        [EnableRateLimiting(RateLimitPolicies.PerUser)]
        [Authorize(Roles = Roles.Customer)]
        public async Task<IActionResult> GetProfileAsync(CancellationToken cancellationToken = default)
        {
            ProfileDto profile = await _sender.Send(new GetProfileQuery(), cancellationToken);

            return Ok(new ApiResponse<ProfileDto>
            {
                Success = true,
                Message = "Lấy thông tin profile thành công",
                Data = profile
            });
        }

        [EnableRateLimiting(RateLimitPolicies.PerUser)]
        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync(CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(Request.Cookies["refreshToken"]))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Cần refresh token để đăng xuất"
                });
            }

            await _sender.Send(new LogoutCommand(Request.Cookies["refreshToken"]!), cancellationToken);

            Response.Cookies.Delete("refreshToken");

            return Ok(new ApiResponse
            {
                Message = "Đăng xuất thành công",
                Success = true
            });
        }

        [EnableRateLimiting(RateLimitPolicies.PerIp)]
        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshAsync(CancellationToken cancellationToken = default)
        {
            string? refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Cần refresh token"
                });
            }

            var result = await _sender.Send(new RefreshCommand(refreshToken), cancellationToken);

            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(_appSettings.JwtConfig!.RefreshTokenExpirationDays)
            });

            return Ok(new AuthResult
            {
                Success = true,
                Message = "Làm mới token thành công",
                AccessToken = result.AccessToken,
            });
        }

        [HttpGet("google/redirect")]
        [AllowAnonymous]
        public IActionResult GoogleRedirect([FromQuery] string returnUrl = "/")
        {
            string state = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

            _memoryCache.Set($"google_oauth_state:{state}", returnUrl, TimeSpan.FromMinutes(5));

            Dictionary<string, string> query = new Dictionary<string, string>
            {
                ["client_id"] = _googleAuthSettings.ClientId,
                ["response_type"] = "code",
                ["scope"] = string.Join(" ", _googleAuthSettings.Scopes ?? new[] { "openid", "email", "profile" }),
                ["redirect_uri"] = _googleAuthSettings.RedirectUri,
                ["state"] = state,
                ["access_type"] = "offline",
                ["prompt"] = "consent"
            };

            string url = QueryHelpers.AddQueryString("https://accounts.google.com/o/oauth2/v2/auth", query!);

            return Redirect(url);
        }

        [AllowAnonymous]
        [HttpGet("google/callback")]
        public async Task<IActionResult> GoogleCallBack([FromQuery] string code, [FromQuery] string state, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(code))
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Cần mã ủy quyền từ Google"
                });

            if (!_memoryCache.TryGetValue<string>($"google_oauth_state:{state}", out var returnUrl))
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Trạng thái đăng nhập Google không hợp lệ hoặc đã hết hạn"
                });

            _memoryCache.Remove($"google_oauth_state:{state}");

            string redirectUri = $"{Request.Scheme}://localhost:7044/api/v1/auth/google/callback";

            var result = await _sender.Send(new LoginWithGoogleCodeCommand(new LoginWithGoogleCodeRequest
            {
                Code = code,
                RedirectUri = redirectUri
            }), cancellationToken);

            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(_appSettings.JwtConfig!.RefreshTokenExpirationDays)
            });

            return Redirect(returnUrl!);
        }
    }
}
