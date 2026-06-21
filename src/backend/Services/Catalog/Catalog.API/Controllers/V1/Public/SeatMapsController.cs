using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Models.Responses;
using Catalog.Application.Common.DTOs.SeatMaps;
using Catalog.Application.Features.SeatMaps.Queries.GetSeatMapById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers.V1.Public
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/venue/{venueId:guid}/seat-maps")]
    [ApiController]
    [AllowAnonymous]
    public class SeatMapsController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly ICacheService _cacheService;

        public SeatMapsController(ISender sender, ICacheService cacheService)
        {
            _sender = sender;
            _cacheService = cacheService;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetSeatMapById(
            [FromRoute] Guid venueId,
            [FromRoute] Guid id,
            CancellationToken cancellation = default)
        {
            string cacheKey = $"catalog:seatmap:id:{id}";

            var result = await _cacheService.GetAsync<SeatMapDto>(cacheKey, cancellation);
            if (result == null)
            {
                result = await _sender.Send(new GetSeatMapByIdQuery(venueId, id), cancellation);
                await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10), cancellation);
            }

            return Ok(new ApiResponse<SeatMapDto>
            {
                Data = result,
                Success = true,
                Message = "Lấy thông tin sơ đồ chỗ ngồi thành công."
            });
        }
    }
}
