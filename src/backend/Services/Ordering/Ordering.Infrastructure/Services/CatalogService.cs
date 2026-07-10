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

        public async Task<CheckoutDataResult> GetCheckoutDataAsync(
            Guid eventId,
            Guid showtimeId,
            List<Ordering.Common.Dtos.CheckoutItemDto> items)
        {
            try
            {
                var request = new GetCheckoutDataRequest
                {
                    EventId = eventId.ToString(),
                    ShowtimeId = showtimeId.ToString(),
                };

                request.Items.AddRange(items.Select(t => new CheckoutRequestItem
                {
                    SeatId = t.SeatId?.ToString() ?? "",
                    TicketTypeId = t.TicketTypeId.ToString(),
                    Quantity = t.Quantity
                }));

                GetCheckoutDataResponse response = await _client.GetCheckoutDataAsync(request);

                if (!response.IsSuccess)
                {
                    return new CheckoutDataResult { IsSuccess = false, Message = response.Message };
                }

                var result = new CheckoutDataResult
                {
                    IsSuccess = true,
                    Message = response.Message,
                    EventTitle = response.EventTitle,
                    OrganizerId = Guid.Parse(response.OrganizerId),
                    ShowtimeStartAt = DateTime.Parse(response.ShowtimeStartAt),
                    ShowtimeEndAt = DateTime.Parse(response.ShowtimeEndAt)
                };

                foreach (var item in response.TicketItems)
                {
                    result.TicketItems.Add(new ValidatedCheckoutTicketItemDto
                    {
                        SeatId = string.IsNullOrEmpty(item.SeatId) ? null : Guid.Parse(item.SeatId),
                        TicketTypeId = Guid.Parse(item.TicketTypeId),
                        TicketTypeName = item.TicketTypeName,
                        Price = (decimal)item.Price,
                        SeatName = item.SeatName,
                        RowName = item.RowName,
                        Quantity = item.Quantity
                    });
                }

                return result;
            }
            catch (RpcException ex)
            {
                return new CheckoutDataResult { IsSuccess = false, Message = $"Lỗi kết nối gRPC tới Catalog: {ex.Status.Detail}" };
            }
        }
    }
}
