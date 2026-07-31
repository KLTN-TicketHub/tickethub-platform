using AI.Infrastructure.Interfaces.IServices;
using Grpc.Core;
using Ordering.API.Protos;

namespace AI.Infrastructure.Services
{
    public class OrderingAiClient : IOrderingAiClient
    {
        private readonly OrderingGrpc.OrderingGrpcClient _client;

        public OrderingAiClient(OrderingGrpc.OrderingGrpcClient client)
        {
            _client = client;
        }

        public async Task<MyOrdersResult> GetMyOrdersAsync(Guid userId, int maxResults)
        {
            try
            {
                GetMyOrdersRequest request = new GetMyOrdersRequest
                {
                    UserId = userId.ToString(),
                    MaxResults = maxResults
                };

                GetMyOrdersResponse response = await _client.GetMyOrdersAsync(request);

                if (!response.IsSuccess)
                    return new MyOrdersResult { IsSuccess = false, Message = response.Message };

                MyOrdersResult result = new MyOrdersResult { IsSuccess = true, Message = response.Message };

                foreach (var o in response.Orders)
                {
                    result.Orders.Add(new OrderSummaryResult
                    {
                        OrderId = Guid.Parse(o.OrderId),
                        EventTitle = o.EventTitle,
                        Status = o.Status,
                        ShowtimeStartAt = DateTime.Parse(o.ShowtimeStartAt),
                        TotalPrice = (decimal)o.TotalPrice,
                        CreatedAt = DateTime.Parse(o.CreatedAt)
                    });
                }

                return result;
            }
            catch (RpcException ex)
            {
                return new MyOrdersResult { IsSuccess = false, Message = $"Lỗi kết nối gRPC tới Ordering: {ex.Status.Detail}" };
            }
        }
    }
}
