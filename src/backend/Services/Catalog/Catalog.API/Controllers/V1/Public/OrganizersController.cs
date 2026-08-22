using BuildingBlocks.Contracts.Models.Responses;
using Catalog.Application.Common.DTOs.Profiles;
using Catalog.Application.Features.Organizers.Queries.GetFeaturedOrganizers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers.V1.Public
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/organizers")]
    [ApiController]
    public class OrganizersController : ControllerBase
    {
        private readonly ISender _sender;

        public OrganizersController(ISender sender)
        {
            _sender = sender;
        }

        [AllowAnonymous]
        [HttpGet("featured")]
        public async Task<IActionResult> GetFeaturedOrganizersAsync([FromQuery] int count = 4, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new GetFeaturedOrganizersQuery(count), cancellationToken);

            return Ok(new ApiResponse<List<FeaturedOrganizerDto>>
            {
                Success = true,
                Message = "Lấy danh sách ban tổ chức nổi bật thành công",
                Data = result,
            });
        }
    }
}
