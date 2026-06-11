using BuildingBlocks.Contracts.Models.Pagination;
using BuildingBlocks.Contracts.Models.Responses;
using Catalog.Application.Common.DTOs.SeatMaps;
using Catalog.Application.Features.SeatMaps.Commands.CreateSeatMap;
using Catalog.Application.Features.SeatMaps.Queries.GetSeatMapById;
using Catalog.Application.Features.SeatMaps.Queries.GetSeatMapsByVenueId;
using Catalog.Application.Features.SeatMaps.Requests;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/venue/{venueId:guid}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SeatMapsController : ControllerBase
    {
        private readonly ISender _sender;

        public SeatMapsController(ISender sender)
        {
            _sender = sender;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateSeatMap(
            [FromRoute] Guid venueId,
            [FromBody] CreateSeatMapRequest request,
            CancellationToken cancellation = default)
        {
            var result = await _sender.Send(new CreateSeatMapCommand(venueId, request), cancellation);

            return Ok(new ApiResponse<SeatMapDto>
            {
                Data = result,
                Success = true,
                Message = "Tạo sơ đồ chỗ ngồi thành công."
            });
        }

        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetSeatMapById(
            [FromRoute] Guid venueId,
            [FromRoute] Guid id,
            CancellationToken cancellation = default)
        {
            var result = await _sender.Send(new GetSeatMapByIdQuery(venueId, id), cancellation);

            return Ok(new ApiResponse<SeatMapDto>
            {
                Data = result,
                Success = true,
                Message = "Lấy thông tin sơ đồ chỗ ngồi thành công."
            });

            throw new NotImplementedException();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetSeatMapsByVenueId(
            [FromRoute] Guid venueId,
            [FromQuery] GetSeatMapsByVenueIdRequest request,
            CancellationToken cancellation = default)
        {
            var result = await _sender.Send(new GetSeatMapsByVenueIdQuery(venueId, request), cancellation);

            return Ok(new ApiResponse<PaginatedResult<SeatMapListItemDto>>
            {
                Data = result,
                Success = true,
                Message = "Lấy danh sách sơ đồ chỗ ngồi thành công."
            });
            throw new NotImplementedException();
        }
    }
}
