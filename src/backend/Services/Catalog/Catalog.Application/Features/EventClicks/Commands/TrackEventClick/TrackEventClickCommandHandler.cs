using Catalog.Application.Common.Interfaces;
using MediatR;

namespace Catalog.Application.Features.EventClicks.Commands.TrackEventClick
{
    public class TrackEventClickCommandHandler : IRequestHandler<TrackEventClickCommand>
    {
        private readonly IEventClickTrackingService _eventClickTrackingService;

        public TrackEventClickCommandHandler(IEventClickTrackingService eventClickTrackingService)
        {
            _eventClickTrackingService = eventClickTrackingService;
        }

        public async Task Handle(TrackEventClickCommand command, CancellationToken cancellation = default)
        {
            await TrackClickAsync(command, cancellation);
        }

        private async Task TrackClickAsync(TrackEventClickCommand command, CancellationToken cancellation = default)
        {
            await _eventClickTrackingService.RecordClickAsync(command.EventId, command.ClickType, command.UserId, cancellation);
        }
    }
}
