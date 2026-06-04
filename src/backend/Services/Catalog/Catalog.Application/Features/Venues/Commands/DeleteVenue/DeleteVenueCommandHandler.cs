using BuildingBlocks.Domain.Exceptions;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using MediatR;

namespace Catalog.Application.Features.Venues.Commands.DeleteVenue
{
    public class DeleteVenueCommandHandler : IRequestHandler<DeleteVenueCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteVenueCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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

            await _unitOfWork.VenueRepository.DeleteAsync(venue, cancellationToken);

            return Unit.Value;
        }
    }
}