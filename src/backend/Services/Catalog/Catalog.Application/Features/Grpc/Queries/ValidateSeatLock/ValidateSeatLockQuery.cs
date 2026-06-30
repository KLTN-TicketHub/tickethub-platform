using Catalog.Application.Features.Grpc.Common;
using MediatR;

namespace Catalog.Application.Features.Grpc.Queries.ValidateSeatLock
{
    public record ValidateSeatLockQuery(
        Guid ShowtimeId,
        List<Guid> SeatIds) : IRequest<GrpcValidationResult>;
}
