using BuildingBlocks.API.Extensions;
using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Contracts.Models.Pagination;
using BuildingBlocks.Contracts.Models.Responses;
using Identity.Application.Common.DTOs.Admin;
using Identity.Application.Features.Admin.Users.Commands.SetUserLockStatus;
using Identity.Application.Features.Admin.Users.Commands.UpdateUser;
using Identity.Application.Features.Admin.Users.Queries.GetUserById;
using Identity.Application.Features.Admin.Users.Queries.GetUsers;
using Identity.Application.Features.Admin.Users.Requests;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Identity.API.Controllers.V1.Admin
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/users")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = Roles.Admin)]
    public class UsersController : ControllerBase
    {
        private readonly ISender _sender;

        public UsersController(ISender sender)
        {
            _sender = sender;
        }

        [EnableRateLimiting(RateLimitPolicies.PerUser)]
        [HttpGet]
        public async Task<IActionResult> GetUsersAsync(
            [FromQuery] GetUsersRequest request,
            CancellationToken cancellationToken = default)
        {
            PaginatedResult<UserListItemDto> result = await _sender.Send(new GetUsersQuery(request), cancellationToken);

            return Ok(new ApiResponse<PaginatedResult<UserListItemDto>>
            {
                Success = true,
                Message = "Lấy danh sách người dùng thành công",
                Data = result
            });
        }

        [EnableRateLimiting(RateLimitPolicies.PerUser)]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetUserByIdAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            UserDetailDto result = await _sender.Send(new GetUserByIdQuery(id), cancellationToken);

            return Ok(new ApiResponse<UserDetailDto>
            {
                Success = true,
                Message = "Lấy chi tiết người dùng thành công",
                Data = result
            });
        }

        [EnableRateLimiting(RateLimitPolicies.PerUser)]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateUserAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateUserRequest request,
            CancellationToken cancellationToken = default)
        {
            UserDetailDto result = await _sender.Send(new UpdateUserCommand(id, request), cancellationToken);

            return Ok(new ApiResponse<UserDetailDto>
            {
                Success = true,
                Message = "Cập nhật thông tin người dùng thành công",
                Data = result
            });
        }

        [EnableRateLimiting(RateLimitPolicies.PerUser)]
        [HttpPatch("{id:guid}/lock-status")]
        public async Task<IActionResult> SetUserLockStatusAsync(
            [FromRoute] Guid id,
            [FromBody] SetUserLockStatusRequest request,
            CancellationToken cancellationToken = default)
        {
            UserDetailDto result = await _sender.Send(new SetUserLockStatusCommand(id, request), cancellationToken);

            return Ok(new ApiResponse<UserDetailDto>
            {
                Success = true,
                Message = request.IsLocked ? "Khoá tài khoản người dùng thành công" : "Mở khoá tài khoản người dùng thành công",
                Data = result
            });
        }
    }
}
