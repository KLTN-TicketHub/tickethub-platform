using BuildingBlocks.API.Extensions;
using BuildingBlocks.Contracts.Models.Responses;
using Identity.Application.Features.Auth.Commands.Logout;
using Identity.Application.Features.Auth.Commands.Refresh;
using Identity.Application.Features.Auth.Request;
using Identity.Common.Options;
using Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Identity.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
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

        [EnableRateLimiting(RateLimitPolicies.PerUser)]
        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync(CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(Request.Cookies["refreshToken"]))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Refresh token is required for logout"
                });
            }

            await _sender.Send(new LogoutCommand(Request.Cookies["refreshToken"]!), cancellationToken);

            Response.Cookies.Delete("refreshToken");

            return Ok(new ApiResponse
            {
                Message = "Logout successful",
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
                    Message = "Refresh token is required"
                });
            }

            var result = await _sender.Send(new RefreshCommand(refreshToken), cancellationToken);

            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            return Ok(new AuthResult
            {
                Success = true,
                Message = "Token refreshed successfully",
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

        [HttpGet]
        public IActionResult GoogleCallBack([FromQuery] string code, [FromQuery] string state)
        {
            //if (!_memoryCache.TryGetValue<string>($"google_oauth_state:{state}", out var returnUrl))
            //    return BadRequest("Invalid or expired state.");

            //_memoryCache.Remove($"google_oauth_state:{state}");

            //var result = await _sender.Send(new LoginWithGoogleCodeCommand(code, returnUrl, HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + "/api/v1/auth/google/callback"));

            //// 3) result should contain app access token and refresh token value (or refresh cookie already set)
            //// We'll redirect to FE returnUrl with access token in fragment to avoid server logs
            //var redirectUri = $"{returnUrl}#access_token={result.AccessToken}&expires_in={result.ExpiresInSeconds}";
            //return Redirect(redirectUri);

            throw new NotImplementedException();
        }
    }
}
