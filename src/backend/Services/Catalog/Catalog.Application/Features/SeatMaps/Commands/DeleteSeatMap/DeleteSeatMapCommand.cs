using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Application.Features.SeatMaps.Commands.DeleteSeatMap
{
    public record DeleteSeatMapCommand(Guid VenueId,Guid Id) : IRequest<Unit>;
}
