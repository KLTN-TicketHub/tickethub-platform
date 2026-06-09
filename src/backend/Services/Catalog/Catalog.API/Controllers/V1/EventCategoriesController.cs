using BuildingBlocks.API.Extensions;
using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Contracts.Models.Responses;
using Catalog.Application.Common.DTOs.EventCategories;
using Catalog.Application.Features.EventCategories.Commands.CreateEventCategory;
using Catalog.Application.Features.EventCategories.Commands.DeleteEventCategory;
using Catalog.Application.Features.EventCategories.Commands.UpdateEventCategory;
using Catalog.Application.Features.EventCategories.Queries.GetEventCategoryById;
using Catalog.Application.Features.EventCategories.Requests;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Catalog.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EventCategoriesController : ControllerBase
    {
        private readonly ISender _sender;

        public EventCategoriesController(ISender sender)
        {
            _sender = sender;
        }

        [EnableRateLimiting(RateLimitPolicies.PerUser)]
        [Authorize(Roles = Roles.Moderator)]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetEventCategoryByIdAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            EventCategoryDto result = await _sender.Send(new GetEventCategoryByIdQuery(id), cancellationToken);

            return Ok(new ApiResponse<EventCategoryDto>
            {
                Success = true,
                Message = "Lấy chi tiết danh mục sự kiện thành công",
                Data = result
            });
        }

        [EnableRateLimiting(RateLimitPolicies.PerUser)]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateEventCategoryAsync([FromBody] CreateEventCategoryRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new CreateEventCategoryCommand(request), cancellationToken);

            return Ok(new ApiResponse<EventCategoryDto>
            {
                Success = true,
                Message = "Tạo danh mục sự kiện thành công",
                Data = result
            });
        }

        [EnableRateLimiting(RateLimitPolicies.PerUser)]
        [AllowAnonymous]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateEventCategoryAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateEventCategoryRequest request,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new UpdateEventCategoryCommand(id, request), cancellationToken);

            return Ok(new ApiResponse<EventCategoryDto>
            {
                Success = true,
                Message = "Cập nhật danh mục sự kiện thành công",
                Data = result
            });
        }

        [EnableRateLimiting(RateLimitPolicies.PerUser)]
        [AllowAnonymous]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteEventCategoryAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            await _sender.Send(new DeleteEventCategoryCommand(id), cancellationToken);

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Xóa danh mục sự kiện thành công"
            });
        }
    }
}
