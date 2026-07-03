using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Commands.Payment;
using BuildingBlocks.Contracts.Events.Payment;
using MassTransit;
using Microsoft.Extensions.Logging;
using Payment.Infrastructure.Entities;
using Payment.Infrastructure.Interfaces;
using Payment.Infrastructure.Interfaces.IServices;

namespace Payment.Infrastructure.Consumers
{
    public class GeneratePaymentLinkConsumer : IConsumer<GeneratePaymentLinkCommand>
    {
        private readonly IVnpayService _vnpayService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GeneratePaymentLinkConsumer> _logger;

        public GeneratePaymentLinkConsumer(
            IVnpayService vnpayService,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            ILogger<GeneratePaymentLinkConsumer> logger)
        {
            _vnpayService = vnpayService;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<GeneratePaymentLinkCommand> context)
        {
            try
            {
                GeneratePaymentLinkCommand msg = context.Message;
                string ipAddress = string.IsNullOrEmpty(_currentUserService.IpAddress) ? "127.0.0.1" : _currentUserService.IpAddress;

                string paymentLink = _vnpayService.CreatePaymentUrl(msg.OrderId, msg.Amount, ipAddress, msg.CustomerName);

                PaymentTransaction transaction = new PaymentTransaction(
                    orderId: msg.OrderId,
                    merchantOrderNo: msg.OrderId.ToString(),
                    amount: msg.Amount,
                    gateway: msg.Gateway
                );
                await _unitOfWork.PaymentTransactionRepository.CreateAsync(transaction);
                await _unitOfWork.SaveChangesAsync();

                await context.Publish(new PaymentLinkGeneratedEvent
                {
                    OrderId = msg.OrderId,
                    PaymentLink = paymentLink,
                    MerchantOrderNo = transaction.MerchantOrderNo
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xảy ra khi tạo payment link cho OrderId: {OrderId}", context.Message.OrderId);
                throw;
            }
        }
    }
}
