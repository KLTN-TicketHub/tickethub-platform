using Catalog.API.Protos;
using Catalog.Application.Features.Grpc.Queries.ValidateCheckout;
using Catalog.Application.Features.Grpc.Queries.ValidateSeatLock;
using Catalog.Application.Features.Grpc.Queries.ValidateTicketTypes;
using Grpc.Core;
using MediatR;

namespace Catalog.API.Services
{
    public class CatalogGrpcService : CatalogGrpc.CatalogGrpcBase
    {
        private readonly IMediator _mediator;

        public CatalogGrpcService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task<ValidateCheckoutResponse> ValidateCheckout(
            ValidateCheckoutRequest request,
            ServerCallContext context)
        {
            try
            {
                if (!Guid.TryParse(request.EventId, out var eventId))
                    return new ValidateCheckoutResponse { IsSuccess = false, Message = "EventId không đúng định dạng Guid." };

                if (!Guid.TryParse(request.ShowtimeId, out var showtimeId))
                    return new ValidateCheckoutResponse { IsSuccess = false, Message = "ShowtimeId không đúng định dạng Guid." };

                List<Guid> seatIds = new List<Guid>();
                foreach (var seatIdStr in request.SeatIds)
                {
                    if (!Guid.TryParse(seatIdStr, out var seatId))
                        return new ValidateCheckoutResponse { IsSuccess = false, Message = $"SeatId '{seatIdStr}' không đúng định dạng Guid." };
                    seatIds.Add(seatId);
                }

                List<(Guid TicketTypeId, int Quantity)> ticketItems = new List<(Guid TicketTypeId, int Quantity)>();
                foreach (var item in request.TicketItems)
                {
                    if (!Guid.TryParse(item.TicketTypeId, out var ticketTypeId))
                        return new ValidateCheckoutResponse { IsSuccess = false, Message = $"TicketTypeId '{item.TicketTypeId}' không đúng định dạng Guid." };
                    ticketItems.Add((ticketTypeId, item.Quantity));
                }

                var query = new ValidateCheckoutQuery(eventId, showtimeId, seatIds, ticketItems);
                var result = await _mediator.Send(query, context.CancellationToken);

                return new ValidateCheckoutResponse
                {
                    IsSuccess = result.IsSuccess,
                    Message = result.Message
                };
            }
            catch (Exception ex)
            {
                return new ValidateCheckoutResponse
                {
                    IsSuccess = false,
                    Message = $"Lỗi xử lý hệ thống: {ex.Message}"
                };
            }
        }

        public override async Task<ValidateSeatLockResponse> ValidateSeatLock(
            ValidateSeatLockRequest request,
            ServerCallContext context)
        {
            try
            {
                if (!Guid.TryParse(request.ShowtimeId, out var showtimeId))
                    return new ValidateSeatLockResponse { IsSuccess = false, Message = "ShowtimeId không đúng định dạng Guid." };

                var seatIds = new List<Guid>();
                foreach (var seatIdStr in request.SeatIds)
                {
                    if (!Guid.TryParse(seatIdStr, out var seatId))
                        return new ValidateSeatLockResponse { IsSuccess = false, Message = $"SeatId '{seatIdStr}' không đúng định dạng Guid." };
                    seatIds.Add(seatId);
                }

                var query = new ValidateSeatLockQuery(showtimeId, seatIds);
                var result = await _mediator.Send(query, context.CancellationToken);

                return new ValidateSeatLockResponse
                {
                    IsSuccess = result.IsSuccess,
                    Message = result.Message
                };
            }
            catch (Exception ex)
            {
                return new ValidateSeatLockResponse
                {
                    IsSuccess = false,
                    Message = $"Lỗi xử lý hệ thống: {ex.Message}"
                };
            }
        }

        public override async Task<ValidateTicketTypesResponse> ValidateTicketTypes(
            ValidateTicketTypesRequest request,
            ServerCallContext context)
        {
            try
            {
                if (!Guid.TryParse(request.ShowtimeId, out var showtimeId))
                    return new ValidateTicketTypesResponse { IsSuccess = false, Message = "ShowtimeId không đúng định dạng Guid." };

                var ticketItems = new List<(Guid TicketTypeId, int Quantity)>();
                foreach (var item in request.TicketItems)
                {
                    if (!Guid.TryParse(item.TicketTypeId, out var ticketTypeId))
                        return new ValidateTicketTypesResponse { IsSuccess = false, Message = $"TicketTypeId '{item.TicketTypeId}' không đúng định dạng Guid." };
                    ticketItems.Add((ticketTypeId, item.Quantity));
                }

                var query = new ValidateTicketTypesQuery(showtimeId, ticketItems);
                var result = await _mediator.Send(query, context.CancellationToken);

                return new ValidateTicketTypesResponse
                {
                    IsSuccess = result.IsSuccess,
                    Message = result.Message
                };
            }
            catch (Exception ex)
            {
                return new ValidateTicketTypesResponse
                {
                    IsSuccess = false,
                    Message = $"Lỗi xử lý hệ thống: {ex.Message}"
                };
            }
        }
    }
}
