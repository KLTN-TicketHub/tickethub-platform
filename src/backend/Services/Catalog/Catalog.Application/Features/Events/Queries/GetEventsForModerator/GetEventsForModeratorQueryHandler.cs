using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Models.Pagination;
using Catalog.Application.Common.DTOs.Events;
using Catalog.Application.Features.Events.Requests;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Interfaces;
using MediatR;
using System.Linq.Expressions;

namespace Catalog.Application.Features.Events.Queries.GetEventsForModerator
{
    public class GetEventsForModeratorQueryHandler : IRequestHandler<GetEventsForModeratorQuery, PaginatedResult<ModeratorEventListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public GetEventsForModeratorQueryHandler(IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        public async Task<PaginatedResult<ModeratorEventListItemDto>> Handle(GetEventsForModeratorQuery query, CancellationToken cancellationToken)
        {
            return await GetEventsForModeratorAsync(query.Request, cancellationToken);
        }

        private async Task<PaginatedResult<ModeratorEventListItemDto>> GetEventsForModeratorAsync(
            GetEventsForModeratorRequest request,
            CancellationToken cancellation = default)
        {
            var search = request.Search?.Trim();

            Expression<Func<Event, bool>> filter;

            if (request.Status == EventStatusForModerator.PendingApproval)
            {
                filter = e => !e.IsDeleted &&
                              (string.IsNullOrEmpty(search) || e.Title.Contains(search)) &&
                              e.Status == EventStatus.PendingApproval;
            }
            else if (request.Status == EventStatusForModerator.Published)
            {
                filter = e => !e.IsDeleted &&
                              (string.IsNullOrEmpty(search) || e.Title.Contains(search)) &&
                              e.Status == EventStatus.Published;
            }
            else if (request.Status == EventStatusForModerator.Rejected)
            {
                filter = e => !e.IsDeleted &&
                              (string.IsNullOrEmpty(search) || e.Title.Contains(search)) &&
                              e.Status == EventStatus.Rejected;
            }
            else
            {
                filter = e => !e.IsDeleted &&
                              (string.IsNullOrEmpty(search) || e.Title.Contains(search));
            }

            (IEnumerable<ModeratorEventListItemDto> events, int totalCount) =
                await _unitOfWork.EventRepository.GetPagedAsync(
                    filter: filter,
                    orderBy: q => q.OrderByDescending(e => e.CreatedAt),
                    selector: e => new ModeratorEventListItemDto
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

            return new PaginatedResult<ModeratorEventListItemDto>(
                events,
                totalCount,
                request.PageNumber,
                request.PageSize);
        }
    }
}
