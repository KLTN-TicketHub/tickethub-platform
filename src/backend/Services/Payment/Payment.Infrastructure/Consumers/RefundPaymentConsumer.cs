using BuildingBlocks.Contracts.Commands.Payment;
using BuildingBlocks.Contracts.Events.Payment;
using MassTransit;
using Microsoft.Extensions.Logging;
using Payment.Infrastructure.Entities;
using Payment.Infrastructure.Interfaces;
using Payment.Infrastructure.Interfaces.IServices;

namespace Payment.Infrastructure.Consumers
{
    public class RefundPaymentConsumer : IConsumer<RefundPaymentCommand>
    {
        private readonly IVnpayService _vnpayService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RefundPaymentConsumer> _logger;

        public RefundPaymentConsumer(IVnpayService vnpayService, IUnitOfWork unitOfWork, ILogger<RefundPaymentConsumer> logger)
        {
            _vnpayService = vnpayService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<RefundPaymentCommand> context)
        {
            RefundPaymentCommand message = context.Message;

            try
            {
                PaymentTransaction? transaction = await _unitOfWork.PaymentTransactionRepository.GetOneAsync<PaymentTransaction>(
                    filter: t => t.OrderId == message.OrderId);

                if (transaction == null || transaction.Status != PaymentStatus.Success || transaction.TransactionId == null || transaction.PayDate == null)
                {
                    _logger.LogWarning("Cannot refund OrderId={OrderId}: no valid successful payment transaction found.", message.OrderId);
                    await context.Publish(new PaymentRefundFailedEvent
                    {
                        OrderId = message.OrderId,
                        Reason = "Không tìm thấy giao dịch thanh toán thành công cho đơn hàng."
                    });
                    return;
                }

                bool isFullRefund = message.Amount >= transaction.Amount;

                (bool isSuccess, string? vnpayRefundTransactionId, string rawResponse, string resultMessage) = await _vnpayService.RefundAsync(
                    orderId: message.OrderId,
                    amount: message.Amount,
                    originalVnpTransactionNo: transaction.TransactionId,
                    originalPayDate: transaction.PayDate,
                    isFullRefund: isFullRefund,
                    createdBy: "system",
                    ipAddress: "127.0.0.1",
                    cancellationToken: context.CancellationToken);

                if (isSuccess)
                {
                    transaction.MarkAsRefunded(rawResponse);
                    await _unitOfWork.PaymentTransactionRepository.UpdateAsync(transaction);
                    await _unitOfWork.SaveChangesAsync();

                    await context.Publish(new PaymentRefundedEvent
                    {
                        OrderId = message.OrderId,
                        VnpayRefundTransactionId = vnpayRefundTransactionId ?? string.Empty,
                        RefundedAmount = message.Amount,
                        RefundedAt = DateTime.UtcNow
                    });

                    _logger.LogInformation("Successfully refunded OrderId={OrderId}, amount={Amount}", message.OrderId, message.Amount);
                }
                else
                {
                    _logger.LogError("VNPay rejected the refund for OrderId={OrderId}: {Message}. RawResponse={RawResponse}", message.OrderId, resultMessage, rawResponse);
                    await context.Publish(new PaymentRefundFailedEvent
                    {
                        OrderId = message.OrderId,
                        Reason = resultMessage
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing RefundPaymentCommand for OrderId={OrderId}", message.OrderId);
                throw;
            }
        }
    }
}
