using BuildingBlocks.Contracts.Models.Pagination;
using Catalog.Application.Common.DTOs.Venues;
using Catalog.Application.Features.Venues.Requests;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using MediatR;
using System.Linq.Expressions;

namespace Catalog.Application.Features.Venues.Queries.GetVenues
{
    public class GetVenuesQueryHandler : IRequestHandler<GetVenuesQuery, PaginatedResult<VenueListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetVenuesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedResult<VenueListItemDto>> Handle(GetVenuesQuery query, CancellationToken cancellationToken)
        {
            return await GetVenuesAsync(query.Request, cancellationToken);
        }

        private async Task<PaginatedResult<VenueListItemDto>> GetVenuesAsync(GetVenuesRequest request, CancellationToken cancellationToken = default)
        {
            (IEnumerable<VenueListItemDto> venues, int totalCount) = await _unitOfWork
                .VenueRepository
                .GetPagedAsync(
                selector: BuildSelector(),
                filter: BuildFilter(request)
            );

            return new PaginatedResult<VenueListItemDto>(venues, totalCount, request.PageNumber, request.PageSize);
        }

        private Expression<Func<Venue, bool>>? BuildFilter(GetVenuesRequest request)
        {
            string? search = request.Search?.ToLower();

            Expression<Func<Venue, bool>>? filter = v =>
                ((string.IsNullOrEmpty(search) ||
                v.VenueName.Contains(search!) ||
                v.VenueCode.Contains(search!) ||
                v.AddressLine.Contains(search!) ||
                v.PhoneNumber.Contains(search!)) &&
                (string.IsNullOrEmpty(request.ProvinceCity) || v.ProvinceCity == request.ProvinceCity) &&
                (string.IsNullOrEmpty(request.District) || v.District == request.District) &&
                !v.IsDeleted);

            return filter;
        }

        private Expression<Func<Venue, VenueListItemDto>> BuildSelector()
        {
            return v => new VenueListItemDto
            {
                Id = v.Id,
                VenueName = v.VenueName,
                VenueCode = v.VenueCode,
                AddressLine = v.AddressLine,
                Ward = v.Ward,
                District = v.District,
                ProvinceCity = v.ProvinceCity,
                SeatMapCount = v.SeatMaps.Where(s => !s.IsDeleted).Count(),
                CreateAt = v.CreatedAt
            };
        }
    }
}
