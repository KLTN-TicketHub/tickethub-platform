using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Extensions;
using BuildingBlocks.Domain.Exceptions;
using Catalog.Application.Common.DTOs.Events;
using Catalog.Domain.Interfaces;
using MediatR;

namespace Catalog.Application.Features.Events.Queries.GetByIdForOrganizer
{
    public class GetByIdForOrganizerQueryHandler : IRequestHandler<GetByIdForOrganizerQuery, EventDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFileService _fileService;

        public GetByIdForOrganizerQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _fileService = fileService;
        }

        public async Task<EventDto> Handle(GetByIdForOrganizerQuery query, CancellationToken cancellationToken = default)
        {
            return await GetEventByIdForOrganizerAsync(query.Id, cancellationToken);
        }

        private async Task<EventDto> GetEventByIdForOrganizerAsync(Guid id, CancellationToken cancellation = default)
        {

            return await _unitOfWork.EventRepository.GetOneUntrackedAsync(
                filter: e => e.Id == id && e.OrganizerId == _currentUserService.UserId && !e.IsDeleted,
                selector: e => new EventDto
                {
                    Id = id,
                    SeatMapId = e.SeatMapId,
                    CategoryId = e.CategoryId,
                    CategoryName = e.Category.CategoryName,
                    Title = e.Title,
                    Slug = e.Slug,
                    Description = e.Description,
                    StartAt = e.StartAt,
                    EndAt = e.EndAt,
                    SaleOpenAt = e.SaleOpenAt,
                    SaleCloseAt = e.SaleCloseAt,
                    CurrencyCode = e.CurrencyCode,
                    CoverImageUrl = _fileService.GetAbsoluteUrl(e.CoverImageUrl),
                    Status = e.Status.GetDisplayName(),
                    CreatedAt = e.CreatedAt,
                    RowVersion = e.RowVersion,
                    Location = new EventLocationDto
                    {
                        VenueName = e.Location.VenueName,

                    }
                }) ?? throw new NotFoundException($"Không tìm thấy sự kiện nòa với id {id}");
        }
    }
}
