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
                    _logger.LogWarning("Không thể hoàn tiền cho OrderId={OrderId}: không tìm thấy giao dịch thanh toán thành công hợp lệ.", message.OrderId);
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

                    _logger.LogInformation("Đã hoàn tiền thành công cho OrderId={OrderId}, số tiền={Amount}", message.OrderId, message.Amount);
                }
                else
                {
                    _logger.LogError("VNPay từ chối hoàn tiền cho OrderId={OrderId}: {Message}. RawResponse={RawResponse}", message.OrderId, resultMessage, rawResponse);
                    await context.Publish(new PaymentRefundFailedEvent
                    {
                        OrderId = message.OrderId,
                        Reason = resultMessage
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý RefundPaymentCommand cho OrderId={OrderId}", message.OrderId);
                throw;
            }
        }
    }
}
