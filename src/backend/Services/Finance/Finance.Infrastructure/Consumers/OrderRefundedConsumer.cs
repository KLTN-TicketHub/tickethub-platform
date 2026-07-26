using BuildingBlocks.Contracts.Events.Order;
using Finance.Infrastructure.Entities;
using Finance.Infrastructure.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Finance.Infrastructure.Consumers
{
    public class OrderRefundedConsumer : IConsumer<OrderRefundedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderRefundedConsumer> _logger;

        public OrderRefundedConsumer(IUnitOfWork unitOfWork, ILogger<OrderRefundedConsumer> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderRefundedEvent> context)
        {
            var message = context.Message;

            try
            {
                WalletTransaction? transaction = await _unitOfWork.WalletTransactionRepository.GetOneAsync<WalletTransaction>(
                    filter: t => t.OrderId == message.OrderId && t.Type == WalletTransactionType.Revenue && t.Status == WalletTransactionStatus.Pending);

                if (transaction == null)
                {
                    _logger.LogWarning("No pending revenue transaction found for OrderId {OrderId} to refund.", message.OrderId);
                    return;
                }

                if (message.RefundedAmount >= transaction.Amount)
                {
                    transaction.Void();
                }
                else
                {
                    transaction.ReduceAmount(message.RefundedAmount);
                }

                await _unitOfWork.WalletTransactionRepository.UpdateAsync(transaction);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Updated revenue transaction for OrderId {OrderId} after refund of {RefundedAmount}.", message.OrderId, message.RefundedAmount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing OrderRefundedEvent for OrderId {OrderId}", message.OrderId);
                throw;
            }
        }
    }
}
