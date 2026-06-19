using BuildingBlocks.API.Helpers;
using BuildingBlocks.Contracts.Models.Responses;
using Catalog.Application.Features.Events.Requests;
using Catalog.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers.V1.Common
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/common/[controller]")]
    [ApiController]
    public class LookupController : ControllerBase
    {
        [HttpGet("event-statuses")]
        public IActionResult GetEventStatuses()
        {
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Lấy danh sách trạng thái sự kiện thành công",
                Data = EnumHelper.ToList<EventStatus>()
            });
        }

        [HttpGet("event-statuses-for-moderator")]
        public IActionResult GetEventStatusesForModerator()
        {
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Lấy danh sách trạng thái sự kiện cho moderator thành công",
                Data = EnumHelper.ToList<EventStatusForModerator>()
            });
        }
    }
}
