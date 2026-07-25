using BuildingBlocks.Contracts.Commands.Order;
using BuildingBlocks.Contracts.Events.Email;
using BuildingBlocks.Contracts.Events.Order;
using MassTransit;
using Microsoft.Extensions.Logging;
using Ordering.Infrastructure.Entities;
using Ordering.Infrastructure.Interfaces;

namespace Ordering.Infrastructure.Consumers
{
    public class FinalizeOrderRefundConsumer : IConsumer<FinalizeOrderRefundCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<FinalizeOrderRefundConsumer> _logger;

        public FinalizeOrderRefundConsumer(IUnitOfWork unitOfWork, ILogger<FinalizeOrderRefundConsumer> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<FinalizeOrderRefundCommand> context)
        {
            var message = context.Message;

            try
            {
                Order? order = await _unitOfWork.OrderRepository.GetByIdAsync(message.OrderId, context.CancellationToken);

                if (order == null || order.Status != OrderStatus.Paid)
                {
                    _logger.LogInformation("Bỏ qua FinalizeOrderRefundCommand cho OrderId={OrderId} vì đơn không ở trạng thái Paid.", message.OrderId);
                    return;
                }

                order.RefundOrder(message.RefundedAmount);
                await _unitOfWork.OrderRepository.UpdateAsync(order);

                await context.Publish(new OrderRefundedEvent
                {
                    OrderId = order.Id,
                    EventId = order.EventId,
                    RefundedAmount = message.RefundedAmount,
                    RefundedAt = DateTime.UtcNow
                });

                await context.Publish(new OrderRefundedEmailEvent
                {
                    OrderId = order.Id,
                    EventTitle = order.EventTitle,
                    CustomerName = order.CustomerName,
                    CustomerEmail = order.CustomerEmail,
                    RefundedAmount = message.RefundedAmount,
                    RefundedAt = DateTime.UtcNow
                });

                await _unitOfWork.SaveChangesAsync(context.CancellationToken);
                _logger.LogInformation("Đã hoàn tất hoàn tiền cho OrderId={OrderId}, số tiền={RefundedAmount}", order.Id, message.RefundedAmount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý FinalizeOrderRefundCommand cho OrderId={OrderId}", message.OrderId);
                throw;
            }
        }
    }
}
