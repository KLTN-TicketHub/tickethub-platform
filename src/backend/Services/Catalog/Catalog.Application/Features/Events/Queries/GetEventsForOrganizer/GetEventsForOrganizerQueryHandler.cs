using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Models.Pagination;
using Catalog.Application.Common.DTOs.Events;
using Catalog.Application.Features.Events.Requests;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Interfaces;
using MediatR;
using System.Linq.Expressions;

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

        private async Task<PaginatedResult<OrganizerEventListItemDto>> GetEventsForOrganizerAsync(
            GetEventsForOrganizerRequest request,
            CancellationToken cancellation = default)
        {
            var organizerId = _currentUserService.UserId;
            var search = request.Search?.Trim();
            var now = DateTime.UtcNow;

            Expression<Func<Event, bool>> filter;

            if (request.Status == EventStatus.PendingApproval)
            {
                filter = e =>
                    !e.IsDeleted &&
                    e.OrganizerId == organizerId &&
                    (string.IsNullOrEmpty(search) || e.Title.Contains(search)) &&
                    e.Status == EventStatus.PendingApproval;
            }
            else if (request.Status == EventStatus.Published)
            {
                filter = e =>
                    !e.IsDeleted &&
                    e.OrganizerId == organizerId &&
                    (string.IsNullOrEmpty(search) || e.Title.Contains(search)) &&
                    e.Status == EventStatus.Published;
            }
            else if (request.Status == EventStatus.Archived)
            {
                filter = e =>
                    !e.IsDeleted &&
                    e.OrganizerId == organizerId &&
                    (string.IsNullOrEmpty(search) || e.Title.Contains(search)) &&
                    e.EndAt < now &&
                    e.Status == EventStatus.Published;
            }
            else if (request.Status == EventStatus.Rejected)
            {
                filter = e =>
                    !e.IsDeleted &&
                    e.OrganizerId == organizerId &&
                    (string.IsNullOrEmpty(search) || e.Title.Contains(search)) &&
                    e.Status == EventStatus.Rejected;
            }
            else if (request.Status == EventStatus.Cancelled)
            {
                filter = e =>
                    !e.IsDeleted &&
                    e.OrganizerId == organizerId &&
                    (string.IsNullOrEmpty(search) || e.Title.Contains(search)) &&
                    e.Status == EventStatus.Cancelled;
            }
            else
            {
                filter = e =>
                    !e.IsDeleted &&
                    e.OrganizerId == organizerId &&
                    (string.IsNullOrEmpty(search) || e.Title.Contains(search));
            }

            (IEnumerable<OrganizerEventListItemDto> events, int totalCount) =
                await _unitOfWork.EventRepository.GetPagedAsync(
                    filter: filter,
                    orderBy: q => q.OrderByDescending(e => e.CreatedAt),
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
                    cancellationToken: cancellation);

            return new PaginatedResult<OrganizerEventListItemDto>(
                events,
                totalCount,
                request.PageNumber,
                request.PageSize);
        }
    }
}
