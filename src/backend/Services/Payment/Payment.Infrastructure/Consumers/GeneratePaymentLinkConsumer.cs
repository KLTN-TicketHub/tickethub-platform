using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Commands.Payment;
using BuildingBlocks.Contracts.Events.Payment;
using MassTransit;
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

        public GeneratePaymentLinkConsumer(IVnpayService vnpayService, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _vnpayService = vnpayService;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }
        public async Task Consume(ConsumeContext<GeneratePaymentLinkCommand> context)
        {
            GeneratePaymentLinkCommand msg = context.Message;
            string ipAddress = _currentUserService.IpAddress!;

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
    }
}
