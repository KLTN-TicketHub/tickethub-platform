using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Contracts.Models.Pagination;
using BuildingBlocks.Contracts.Models.Responses;
using Catalog.Application.Common.DTOs.Events;
using Catalog.Application.Features.Events.Commands.CreateEvent;
using Catalog.Application.Features.Events.Queries.GetByIdForOrganizer;
using Catalog.Application.Features.Events.Queries.GetEventsForOrganizer;
using Catalog.Application.Features.Events.Requests;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EventsController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly ICurrentUserService _currentUserService;

        public EventsController(ISender sender, ICurrentUserService currentUserService)
        {
            _sender = sender;
            _currentUserService = currentUserService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateEventAsync(
            [FromBody] CreateEventRequest request,
            CancellationToken cancellationToken = default)
        {
            Guid organizerId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("Không thể xác định danh tính người dùng.");

            EventDto result = await _sender.Send(new CreateEventCommand(organizerId, request), cancellationToken);

            return Ok(new ApiResponse<EventDto>
            {
                Success = true,
                Message = "Tạo sự kiện thành công.",
                Data = result
            });
        }

        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetEventByIdForOrganizerAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            EventDto result = await _sender.Send(new GetByIdForOrganizerQuery(id), cancellationToken);

            return Ok(new ApiResponse<EventDto>
            {
                Success = true,
                Message = "Lấy thông tin sự kiện thành công.",
                Data = result
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetEventsAsync(
            [FromQuery] GetEventsForOrganizerRequest request,
            CancellationToken cancellationToken = default)
        {
            PaginatedResult<OrganizerEventListItemDto> result;
            if (_currentUserService.Roles.Contains(Roles.Organizer))
            {
                result = await _sender.Send(new GetEventsForOrganizerQuery(request), cancellationToken);
            }
            else if (_currentUserService.Roles.Contains(Roles.Customer))
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new UnauthorizedAccessException("Người dùng không có quyền truy cập.");
            }

            return Ok(new ApiResponse<PaginatedResult<OrganizerEventListItemDto>>
            {
                Success = true,
                Message = "Lấy danh sách sự kiện thành công.",
                Data = result
            });
        }
    }
}
