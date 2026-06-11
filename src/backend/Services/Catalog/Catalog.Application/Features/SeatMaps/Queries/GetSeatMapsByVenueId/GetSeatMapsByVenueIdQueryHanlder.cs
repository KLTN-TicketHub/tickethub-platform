using BuildingBlocks.Contracts.Models.Pagination;
using Catalog.Application.Common.DTOs.SeatMaps;
using Catalog.Domain.Interfaces;
using MediatR;

namespace Catalog.Application.Features.SeatMaps.Queries.GetSeatMapsByVenueId
{
    public class GetSeatMapsByVenueIdQueryHandler : IRequestHandler<GetSeatMapsByVenueIdQuery, PaginatedResult<SeatMapListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSeatMapsByVenueIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<PaginatedResult<SeatMapListItemDto>> Handle(GetSeatMapsByVenueIdQuery request, CancellationToken cancellationToken)
        {
            return await GetSeatMapsByVenueIdAsync(request.VenueId, request.Request, cancellationToken);
        }

        private async Task<PaginatedResult<SeatMapListItemDto>> GetSeatMapsByVenueIdAsync(
            Guid venueId,
            PaginatedRequest request,
            CancellationToken cancellation = default)
        {
            (IEnumerable<SeatMapListItemDto> seatMaps, int count) = await _unitOfWork.SeatMapRepository.GetPagedAsync(
                filter: sm => sm.VenueId == venueId && !sm.IsDeleted,
                selector: sm => new SeatMapListItemDto
                {
                    Id = sm.Id,
                    VenueId = sm.VenueId,
                    SeatMapName = sm.SeatMapName,
                    SeatMapCode = sm.SeatMapCode,
                },
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                cancellationToken: cancellation
            );

            return new PaginatedResult<SeatMapListItemDto>(seatMaps, count, request.PageNumber, request.PageSize);
        }
    }
}
