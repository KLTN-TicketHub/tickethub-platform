using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Domain.Exceptions;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using MediatR;

namespace Catalog.Application.Features.Venues.Commands.DeleteVenue
{
    public class DeleteVenueCommandHandler : IRequestHandler<DeleteVenueCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public DeleteVenueCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(DeleteVenueCommand command, CancellationToken cancellationToken)
        {
            return await DeleteVenueAsync(command.Id, cancellationToken);
        }

        private async Task<Unit> DeleteVenueAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Venue venue = await _unitOfWork.VenueRepository.GetOneUntrackedAsync<Venue>(
                filter: v => v.Id == id && !v.IsDeleted,
                cancellation: cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy địa điểm");

            int seatMapCount = await _unitOfWork.VenueRepository.GetCountAsync(v => v.SeatMaps.Any(sm => sm.VenueId == id));

            if (seatMapCount > 0 && !_currentUserService.Roles.Any(r => r == Roles.Admin))
                throw new BusinessRuleException("Cần quyền admin để xóa địa điểm có sơ đồ chỗ ngồi liên kết.");

            await _unitOfWork.VenueRepository.DeleteAsync(venue, cancellationToken);

            return Unit.Value;
        }
    }
}