using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Models.Pagination;
using Catalog.Application.Common.DTOs.Events;
using Catalog.Application.Features.Events.Requests;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using MediatR;
using System.Linq.Expressions;

namespace Catalog.Application.Features.Events.Queries.GetAdminEvents
{
    public class GetAdminEventsQueryHandler : IRequestHandler<GetAdminEventsQuery, PaginatedResult<AdminEventListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public GetAdminEventsQueryHandler(IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        public async Task<PaginatedResult<AdminEventListItemDto>> Handle(GetAdminEventsQuery query, CancellationToken cancellationToken)
        {
            return await GetAdminEventsAsync(query.Request, cancellationToken);
        }

        private async Task<PaginatedResult<AdminEventListItemDto>> GetAdminEventsAsync(
            GetAdminEventsRequest request,
            CancellationToken cancellationToken = default)
        {
            var search = request.Search?.Trim();

            Expression<Func<Event, bool>> filter = e =>
                !e.IsDeleted &&
                (string.IsNullOrEmpty(search) || e.Title.Contains(search)) &&
                (request.Status == null || e.Status == request.Status) &&
                (request.CategoryId == null || e.CategoryId == request.CategoryId);

            (IEnumerable<AdminEventListItemDto> events, int totalCount) =
                await _unitOfWork.EventRepository.GetPagedAsync(
                    filter: filter,
                    orderBy: q => q.OrderByDescending(e => e.CreatedAt),
                    selector: e => new AdminEventListItemDto
                    {
                        Id = e.Id,
                        Title = e.Title,
                        Slug = e.Slug,
                        CoverImageUrl = _fileService.GetAbsoluteUrl(e.CoverImageUrl),
                        Status = e.Status.ToString(),
                        CategoryName = e.Category!.CategoryName,
                        OrganizerName = e.Organizer!.OrganizerName,
                        MinPrice = e.ShowTimes.SelectMany(st => st.TicketTypes).Select(tt => (decimal?)tt.Price).Min() ?? 0,
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
                    cancellationToken: cancellationToken);

            return new PaginatedResult<AdminEventListItemDto>(
                events,
                totalCount,
                request.PageNumber,
                request.PageSize);
        }
    }
}
