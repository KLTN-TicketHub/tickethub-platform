using BuildingBlocks.Contracts.Models.Pagination;
using Catalog.Application.Common.DTOs.EventRatings;
using Catalog.Application.Features.EventRatings.Requests;
using MediatR;

namespace Catalog.Application.Features.EventRatings.Queries.GetEventRatingsForOrganizer
{
    public class GetEventRatingsForOrganizerQuery : IRequest<PaginatedResult<EventRatingDto>>
    {
        public Guid EventId { get; set; }
        public GetEventRatingsRequest Request { get; set; }

        public GetEventRatingsForOrganizerQuery(Guid eventId, GetEventRatingsRequest request)
        {
            EventId = eventId;
            Request = request;
        }
    }
}
