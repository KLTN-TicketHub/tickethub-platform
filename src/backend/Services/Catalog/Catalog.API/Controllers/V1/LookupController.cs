using BuildingBlocks.API.Helpers;
using BuildingBlocks.Contracts.Models.Responses;
using Catalog.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
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
    }
}
