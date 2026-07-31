using Grpc.Core;
using Ordering.API.Protos;
using Ordering.Infrastructure.Entities;
using Ordering.Infrastructure.Interfaces;

namespace Ordering.API.Services
{
    public class OrderingGrpcService : OrderingGrpc.OrderingGrpcBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderingGrpcService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override async Task<GetMyOrdersResponse> GetMyOrders(
            GetMyOrdersRequest request,
            ServerCallContext context)
        {
            try
            {
                if (!Guid.TryParse(request.UserId, out var userId))
                    return new GetMyOrdersResponse { IsSuccess = false, Message = "UserId không đúng định dạng Guid." };

                int maxResults = request.MaxResults > 0 ? request.MaxResults : 5;

                (IEnumerable<Order> orders, _) = await _unitOfWork.OrderRepository.GetPagedAsync(
                    filter: o => o.UserId == userId && !o.IsDeleted,
                    orderBy: q => q.OrderByDescending(o => o.CreatedAt),
                    pageNumber: 1,
                    pageSize: maxResults,
                    cancellationToken: context.CancellationToken);

                GetMyOrdersResponse response = new GetMyOrdersResponse
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách đơn hàng thành công."
                };

                foreach (var o in orders)
                {
                    response.Orders.Add(new OrderSummaryItem
                    {
                        OrderId = o.Id.ToString(),
                        EventTitle = o.EventTitle,
                        Status = o.Status.ToString(),
                        ShowtimeStartAt = o.ShowtimeStartAt.ToString("O"),
                        TotalPrice = (double)o.TotalPrice,
                        CreatedAt = o.CreatedAt.ToString("O")
                    });
                }

                return response;
            }
            catch (Exception ex)
            {
                return new GetMyOrdersResponse
                {
                    IsSuccess = false,
                    Message = $"Lỗi xử lý hệ thống: {ex.Message}"
                };
            }
        }
    }
}
