using Catalog.API.Protos;
using Grpc.Core;
using Inventory.Infrastructure.Interfaces.IServices;

namespace Inventory.Infrastructure.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly CatalogGrpc.CatalogGrpcClient _client;

        public CatalogService(CatalogGrpc.CatalogGrpcClient client)
        {
            _client = client;
        }

        public async Task<(bool IsSuccess, string Message)> ValidateSeatLockAsync(
            Guid showtimeId,
            List<Guid> seatIds)
        {
            try
            {
                var request = new ValidateSeatLockRequest
                {
                    ShowtimeId = showtimeId.ToString()
                };
                request.SeatIds.AddRange(seatIds.Select(id => id.ToString()));

                ValidateSeatLockResponse response = await _client.ValidateSeatLockAsync(request);
                return (response.IsSuccess, response.Message);
            }
            catch (RpcException ex)
            {
                return (false, $"Lỗi kết nối gRPC tới Catalog (ValidateSeatLock): {ex.Status.Detail}");
            }
        }

        public async Task<(bool IsSuccess, string Message)> ValidateTicketTypesAsync(
            Guid showtimeId,
            List<(Guid TicketTypeId, int Quantity)> ticketItems)
        {
            try
            {
                var request = new ValidateTicketTypesRequest
                {
                    ShowtimeId = showtimeId.ToString()
                };
                request.TicketItems.AddRange(ticketItems.Select(t => new TicketTypeQuantityItem
                {
                    TicketTypeId = t.TicketTypeId.ToString(),
                    Quantity = t.Quantity
                }));

                ValidateTicketTypesResponse response = await _client.ValidateTicketTypesAsync(request);
                return (response.IsSuccess, response.Message);
            }
            catch (RpcException ex)
            {
                return (false, $"Lỗi kết nối gRPC tới Catalog (ValidateTicketTypes): {ex.Status.Detail}");
            }
        }
    }
}
