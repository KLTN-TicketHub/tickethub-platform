using Catalog.Application.Common.DTOs.EventRatings;
using MediatR;

namespace Catalog.Application.Features.EventRatings.Queries.GetMyEventRating
{
    public record GetMyEventRatingQuery(Guid EventId, Guid UserId) : IRequest<EventRatingDto?>;
}
