using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Models.Pagination;
using Catalog.Application.Common.DTOs.Events;
using Catalog.Application.Features.Events.Requests;
using Catalog.Domain.Enums;
using Catalog.Domain.Interfaces;
using MediatR;

namespace Catalog.Application.Features.Events.Queries.GetEventsForOrganizer
{
    public class GetEventsForOrganizerQueryHandler :
        IRequestHandler<GetEventsForOrganizerQuery, PaginatedResult<OrganizerEventListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFileService _fileService;

        public GetEventsForOrganizerQueryHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _fileService = fileService;
        }

        public async Task<PaginatedResult<OrganizerEventListItemDto>> Handle(
            GetEventsForOrganizerQuery query,
            CancellationToken cancellationToken = default)
        {
            return await GetEventsForOrganizerAsync(query.Request, cancellationToken);
        }

        private async Task<PaginatedResult<OrganizerEventListItemDto>> GetEventsForOrganizerAsync(GetEventsForOrganizerRequest request,
            CancellationToken cancellation = default)
        {
            string? search = request.Search?.ToLower();

            (IEnumerable<OrganizerEventListItemDto> events, int totalCount) = await _unitOfWork.EventRepository.GetPagedAsync(
                filter: e =>
                    !e.IsDeleted &&
                    e.OrganizerId == _currentUserService.UserId &&
                    (string.IsNullOrEmpty(search) || e.Title.ToLower().Contains(search)) &&
                    (
                        request.Status == EventStatus.PendingApproval ||
                        request.Status == EventStatus.Published
                            ? e.Status == request.Status
                                    : request.Status == EventStatus.Archived
                        ? e.EndAt < DateTime.UtcNow
                        : true
                ),
                orderBy: e => e.OrderBy(e => e.CreatedAt),
                selector: e => new OrganizerEventListItemDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    CoverImageUrl = _fileService.GetAbsoluteUrl(e.CoverImageUrl),
                    Status = e.Status.ToString(),
                    StartAt = e.StartAt,
                    EndAt = e.EndAt,
                    Location = new EventLocationDto
                    {
                        VenueName = e.Location.VenueName,
                        AddressLine = e.Location.AddressLine,
                        Ward = e.Location.Ward,
                        District = e.Location.District,
                        ProvinceCity = e.Location.ProvinceCity,
                        Country = e.Location.Country
                    }
                },
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                cancellationToken: cancellation
            );

            return new PaginatedResult<OrganizerEventListItemDto>(events, totalCount, request.PageNumber, request.PageSize);
        }
    }
}
