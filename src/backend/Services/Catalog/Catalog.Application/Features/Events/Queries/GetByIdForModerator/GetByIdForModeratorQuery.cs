using Catalog.Application.Common.DTOs.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Application.Features.Events.Queries.GetByIdForModerator
{
    public record GetByIdForModeratorQuery(Guid Id) : IRequest<EventDto>;
}
