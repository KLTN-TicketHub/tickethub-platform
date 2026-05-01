using BuildingBlocks.API.Extensions;
using BuildingBlocks.Contracts.Models.Responses;
using Identity.Application.Features.Auth.Commands.Logout;
using Identity.Application.Features.Auth.Commands.Refresh;
using Identity.Application.Features.Auth.Request;
using Identity.Common.Options;
using Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Options;
using System.Security.Claims;

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
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AuthController(IOptions<AppSettings> appSettings, ISender sender)
        {
            _appSettings = appSettings.Value;
            _sender = sender;
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

            await _sender.Send(new LogoutCommand(Request.Cookies["refreshToken"]), cancellationToken);

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

        //[AllowAnonymous]
        //[HttpGet("google-login")]
        //public IActionResult GoogleLogin()
        //{
        //    string? redirectUrl = Url.Action("GoogleResponse", "Auth", new { ReturnUrl = "/" });

        //    var properties = _signInManager.ConfigureExternalAuthenticationProperties(
        //        GoogleDefaults.AuthenticationScheme,
        //        redirectUrl);

        //    return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        //}

        //[HttpGet("google-response")]
        //public async Task<IActionResult> GoogleResponse()
        //{
        //    var info = await _signInManager.GetExternalLoginInfoAsync();

        //    if (info == null)
        //        return BadRequest("Login failed");

        //    var user = await _userManager.FindByLoginAsync(
        //        info.LoginProvider,
        //        info.ProviderKey);

        //    if (user == null)
        //    {
        //         🔥 REGISTER nếu chưa có
        //        var email = info.Principal.FindFirstValue(ClaimTypes.Email);

        //        user = new ApplicationUser
        //        {
        //            UserName = email,
        //            Email = email
        //        };

        //        await _userManager.CreateAsync(user);

        //        await _userManager.AddLoginAsync(user, info);
        //    }

        //     🔥 tạo JWT
        //    var token = _jwtService.GenerateToken(user);

        //    return Redirect($"http://localhost:5173/auth/callback?token={token}");
        //}
    }
}
