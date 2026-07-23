using Grpc.Core;
using Inventory.API.Protos;
using Ordering.Infrastructure.Interfaces.IServices;

namespace Ordering.Infrastructure.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly InventoryGrpc.InventoryGrpcClient _client;

        public InventoryService(InventoryGrpc.InventoryGrpcClient client)
        {
            _client = client;
        }

        public async Task<(bool IsSuccess, string Message)> UpgradeSeatLocksAsync(Guid showtimeId, List<Guid> seatIds, Guid userId)
        {
            try
            {
                UpgradeSeatLocksRequest request = new UpgradeSeatLocksRequest
                {
                    ShowtimeId = showtimeId.ToString(),
                    UserId = userId.ToString()
                };

                request.SeatIds.AddRange(seatIds.Select(id => id.ToString()));

                UpgradeSeatLocksResponse response = await _client.UpgradeSeatLocksAsync(request);

                return (response.IsSuccess, response.Message);
            }
            catch (RpcException ex)
            {
                return (false, $"Lỗi kết nối gRPC tới Inventory: {ex.Status.Detail}");
            }
        }

        public async Task<(bool IsSuccess, string Message)> LockTicketQuantitiesAsync(Guid showtimeId, List<(Guid TicketTypeId, int Quantity)> items, Guid userId)
        {
            try
            {
                LockTicketQuantitiesRequest request = new LockTicketQuantitiesRequest
                {
                    ShowtimeId = showtimeId.ToString(),
                    UserId = userId.ToString()
                };

                request.TicketQuantities.AddRange(items.Select(x => new TicketQuantityDto
                {
                    TicketTypeId = x.TicketTypeId.ToString(),
                    Quantity = x.Quantity
                }));

                LockTicketQuantitiesResponse response = await _client.LockTicketQuantitiesAsync(request);

                return (response.IsSuccess, response.Message);
            }
            catch (RpcException ex)
            {
                return (false, $"Lỗi kết nối gRPC tới Inventory: {ex.Status.Detail}");
            }
        }
    }
}
