using AutoMapper;
using BuildingBlocks.Domain.Exceptions;
using Catalog.Application.Common.DTOs.EventRatings;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using MediatR;

namespace Catalog.Application.Features.EventRatings.Commands.CreateEventRating
{
    public class CreateEventRatingCommandHandler : IRequestHandler<CreateEventRatingCommand, EventRatingDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateEventRatingCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<EventRatingDto> Handle(CreateEventRatingCommand command, CancellationToken cancellation = default)
        {
            return await CreateEventRatingAsync(command, cancellation);
        }

        private async Task<EventRatingDto> CreateEventRatingAsync(CreateEventRatingCommand command, CancellationToken cancellation = default)
        {
            Event @event = await _unitOfWork.EventRepository.GetOneUntrackedAsync<Event>(
                filter: e => e.Id == command.EventId && !e.IsDeleted,
                cancellation: cancellation)
                ?? throw new NotFoundException($"Không tìm thấy sự kiện với ID {command.EventId}.");

            await CheckEligibilityAsync(command.EventId, command.UserId, cancellation);
            await CheckNotAlreadyRatedAsync(command.EventId, command.UserId, cancellation);

            EventRating eventRating = new EventRating(
                command.EventId,
                command.UserId,
                command.ReviewerName,
                command.Request.SoundRating,
                command.Request.VisualRating,
                command.Request.OrganizationRating,
                command.Request.FacilityRating,
                command.Request.ServiceRating,
                command.Request.PerformanceRating,
                command.Request.Comment);

            EventRating created = await _unitOfWork.EventRatingRepository.CreateAsync(eventRating, cancellation);

            return _mapper.Map<EventRatingDto>(created);
        }

        private async Task CheckEligibilityAsync(Guid eventId, Guid userId, CancellationToken cancellation = default)
        {
            EventCheckIn? checkIn = await _unitOfWork.EventCheckInRepository.GetOneUntrackedAsync<EventCheckIn>(
                filter: c => c.EventId == eventId && c.UserId == userId,
                cancellation: cancellation);

            if (checkIn == null)
                throw new BusinessRuleException("Bạn cần check-in tham dự sự kiện này mới có thể đánh giá.");
        }

        private async Task CheckNotAlreadyRatedAsync(Guid eventId, Guid userId, CancellationToken cancellation = default)
        {
            EventRating? existing = await _unitOfWork.EventRatingRepository.GetOneUntrackedAsync<EventRating>(
                filter: r => r.EventId == eventId && r.UserId == userId,
                cancellation: cancellation);

            if (existing != null)
                throw new BusinessRuleException("Bạn đã đánh giá sự kiện này rồi.");
        }
    }
}
