using AutoMapper;
using BuildingBlocks.Domain.Exceptions;
using Catalog.Application.Common.DTOs.Venues;
using Catalog.Application.Features.Venues.Requests;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using MediatR;

namespace Catalog.Application.Features.Venues.Commands.UpdateVenue
{
    public class UpdateVenueCommandHandler : IRequestHandler<UpdateVenueCommand, VenueDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateVenueCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<VenueDto> Handle(UpdateVenueCommand command, CancellationToken cancellationToken)
        {
            return await UpdateVenueAsync(command.Id, command.Request, cancellationToken);
        }

        private async Task<VenueDto> UpdateVenueAsync(Guid id, UpdateVenueRequest request, CancellationToken cancellationToken = default)
        {
            Venue venue = await _unitOfWork.VenueRepository.GetOneUntrackedAsync<Venue>(
                filter: v => v.Id == id,
                cancellation: cancellationToken) ?? throw new NotFoundException("Không tìm thấy địa điểm");

            _mapper.Map(request, venue);

            Venue updatedVenue = await _unitOfWork.VenueRepository.UpdateAsync(venue, cancellationToken);

            return _mapper.Map<VenueDto>(updatedVenue);
        }
    }
}
