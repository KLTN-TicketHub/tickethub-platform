using AutoMapper;
using BuildingBlocks.Domain.Exceptions;
using Catalog.Application.Common.DTOs.Venues;
using Catalog.Domain.Interfaces;
using MediatR;

namespace Catalog.Application.Features.Venues.Queries.GetVenueById
{
    public class GetVenueByIdQueryHandler : IRequestHandler<GetVenueByIdQuery, VenueDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetVenueByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<VenueDto> Handle(GetVenueByIdQuery query, CancellationToken cancellationToken)
        {
            return await GetVenueByIdAsync(query.Id, cancellationToken);
        }

        private async Task<VenueDto> GetVenueByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork
                .VenueRepository
                .GetOneUntrackedAsync(
                filter: v => v.Id == id && !v.IsDeleted,
                selector: v => new VenueDto
                {
                    Id = id,
                    VenueName = v.VenueName,
                    VenueCode = v.VenueCode,
                    AddressLine = v.AddressLine,
                    Ward = v.Ward,
                    District = v.District,
                    ProvinceCity = v.ProvinceCity,
                    Country = v.Country,
                    Slug = v.Slug,
                    PhoneNumber = v.PhoneNumber,
                    WebsiteUrl = v.WebsiteUrl,
                    CreatedAt = v.CreatedAt,
                    RowVersion = v.RowVersion
                },
                cancellation: cancellationToken) ?? throw new NotFoundException("Không tìm thấy địa điểm");
        }
    }
}
