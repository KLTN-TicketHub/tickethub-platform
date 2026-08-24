using AutoMapper;
using Catalog.Application.Common.DTOs.EventRatings;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using MediatR;

namespace Catalog.Application.Features.EventRatings.Queries.GetMyEventRating
{
    public class GetMyEventRatingQueryHandler : IRequestHandler<GetMyEventRatingQuery, EventRatingDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetMyEventRatingQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<EventRatingDto?> Handle(GetMyEventRatingQuery query, CancellationToken cancellation = default)
        {
            return await GetMyEventRatingAsync(query, cancellation);
        }

        private async Task<EventRatingDto?> GetMyEventRatingAsync(GetMyEventRatingQuery query, CancellationToken cancellation = default)
        {
            EventRating? rating = await _unitOfWork.EventRatingRepository.GetOneUntrackedAsync<EventRating>(
                filter: r => r.EventId == query.EventId && r.UserId == query.UserId,
                cancellation: cancellation);

            return rating == null ? null : _mapper.Map<EventRatingDto>(rating);
        }
    }
}
