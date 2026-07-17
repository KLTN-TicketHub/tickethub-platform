using Catalog.Domain.Enums;
using MediatR;

namespace Catalog.Application.Features.EventClicks.Commands.TrackEventClick
{
    public record TrackEventClickCommand(Guid EventId, Guid? UserId, EventClickType ClickType) : IRequest;
}
