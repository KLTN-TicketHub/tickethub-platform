using Catalog.Application.Common.DTOs.EventRatings;
using Catalog.Application.Features.EventRatings.Requests;
using MediatR;

namespace Catalog.Application.Features.EventRatings.Commands.CreateEventRating
{
    public record CreateEventRatingCommand(Guid EventId, Guid UserId, string ReviewerName, CreateEventRatingRequest Request) : IRequest<EventRatingDto>;
}
