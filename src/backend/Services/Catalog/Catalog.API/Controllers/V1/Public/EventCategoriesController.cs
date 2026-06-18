using BuildingBlocks.API.Extensions;
using BuildingBlocks.Contracts.Models.Pagination;
using BuildingBlocks.Contracts.Models.Responses;
using Catalog.Application.Common.DTOs.EventCategories;
using Catalog.Application.Features.EventCategories.Queries.GetEventCategories;
using Catalog.Application.Features.EventCategories.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Catalog.API.Controllers.V1.Public
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/event-categories")]
    [ApiController]
    public class EventCategoriesController : ControllerBase
    {
        private readonly ISender _sender;

        public EventCategoriesController(ISender sender)
        {
            _sender = sender;
        }

        [EnableRateLimiting(RateLimitPolicies.PerIp)]
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetEventCategoriesAsync(
            [FromQuery] GetCategoriesRequest request,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new GetEventCategoriesQuery(request), cancellationToken);
            return Ok(new ApiResponse<PaginatedResult<EventCategoryDto>>
            {
                Success = true,
                Message = "Lấy danh sách danh mục sự kiện thành công",
                Data = result
            });
        }
    }
}
