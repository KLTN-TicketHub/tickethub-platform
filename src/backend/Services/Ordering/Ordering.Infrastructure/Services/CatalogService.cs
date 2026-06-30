using Catalog.API.Protos;
using Grpc.Core;
using Ordering.Infrastructure.Interfaces.IServices;

namespace Ordering.Infrastructure.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly CatalogGrpc.CatalogGrpcClient _client;

        public CatalogService(CatalogGrpc.CatalogGrpcClient client)
        {
            _client = client;
        }

        public async Task<(bool IsSuccess, string Message)> ValidateCheckoutAsync(
            Guid eventId,
            Guid showtimeId,
            List<Guid> seatIds,
            List<CheckoutTicketValidationItem> ticketItems)
        {
            try
            {
                var request = new ValidateCheckoutRequest
                {
                    EventId = eventId.ToString(),
                    ShowtimeId = showtimeId.ToString(),
                };

                request.SeatIds.AddRange(seatIds.Select(id => id.ToString()));
                request.TicketItems.AddRange(ticketItems.Select(t => new CheckoutTicketItem
                {
                    TicketTypeId = t.TicketTypeId.ToString(),
                    Quantity = t.Quantity
                }));

                ValidateCheckoutResponse response = await _client.ValidateCheckoutAsync(request);
                return (response.IsSuccess, response.Message);
            }
            catch (RpcException ex)
            {
                return (false, $"Lỗi kết nối gRPC tới Catalog: {ex.Status.Detail}");
            }
        }
    }
}
