using Catalog.Application.Features.Grpc.Common;
using MediatR;

namespace Catalog.Application.Features.Grpc.Queries.ValidateTicketTypes
{
    public record ValidateTicketTypesQuery(
        Guid ShowtimeId,
        List<(Guid TicketTypeId, int Quantity)> TicketItems) : IRequest<GrpcValidationResult>;
}
