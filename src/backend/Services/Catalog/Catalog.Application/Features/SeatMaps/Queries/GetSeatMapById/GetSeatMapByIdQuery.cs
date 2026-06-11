using Catalog.Application.Common.DTOs.SeatMaps;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Application.Features.SeatMaps.Queries.GetSeatMapById
{
    public record GetSeatMapByIdQuery(Guid VenueId, Guid Id) : IRequest<SeatMapDto>;
}
