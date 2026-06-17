using BuildingBlocks.Domain.Exceptions;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.SeatMaps.Commands.DeleteSeatMap
{
    public class DeleteSeatMapCommandHandler : IRequestHandler<DeleteSeatMapCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteSeatMapCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteSeatMapCommand request, CancellationToken cancellationToken)
        {
            return await DeleteSeatMapAsync(request.VenueId, request.Id, cancellationToken);
        }

        private async Task<Unit> DeleteSeatMapAsync(Guid venueId, Guid id, CancellationToken cancellationToken = default)
        {
            SeatMap seatMap = await _unitOfWork.SeatMapRepository.GetOneUntrackedAsync<SeatMap>(
                s => s.Id == id && s.VenueId == venueId && !s.IsDeleted,
                include: s => s.Include(s => s.Zones).ThenInclude(z => z.Rows).ThenInclude(r => r.Seats),
                cancellation: cancellationToken) ?? throw new NotFoundException($"Không tìm thấy bản đồ ghế với ID {id}");

            await _unitOfWork.SeatMapRepository.DeleteAsync(seatMap, cancellationToken);

            return Unit.Value;
        }
    }
}
