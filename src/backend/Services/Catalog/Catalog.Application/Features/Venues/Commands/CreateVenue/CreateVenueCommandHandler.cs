using AutoMapper;
using Catalog.Application.Common.DTOs.Venues;
using Catalog.Application.Features.Venues.Requests;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using MediatR;

namespace Catalog.Application.Features.Venues.Commands.CreateVenue
{
    public class CreateVenueCommandHandler : IRequestHandler<CreateVenueCommand, VenueDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateVenueCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<VenueDto> Handle(CreateVenueCommand command, CancellationToken cancellation = default)
        {
            return await CreateVenueAsync(command.Request, cancellation);
        }

        private async Task<VenueDto> CreateVenueAsync(CreateVenueRequest request, CancellationToken cancellation = default)
        {
            string venueCode = await _unitOfWork
                .VenueRepository
                .GenerateNextVenueCodeAsync(request.VenueName, cancellation);

            Venue venue = _mapper.Map<Venue>(request);

            venue.SetVenueCode(venueCode);

            return _mapper.Map<VenueDto>(
                await _unitOfWork.VenueRepository.CreateAsync(venue, cancellation));
        }
    }
}
