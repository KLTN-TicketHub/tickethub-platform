using Catalog.Application.Features.Grpc.Common;
using MediatR;

namespace Catalog.Application.Features.Grpc.Queries.ValidateCheckout
{
    public record ValidateCheckoutQuery(
        Guid EventId,
        Guid ShowtimeId,
        List<Guid> SeatIds,
        List<(Guid TicketTypeId, int Quantity)> TicketItems) : IRequest<GrpcValidationResult>;
}
