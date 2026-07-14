using BuildingBlocks.Contracts.Events.Order;
using Finance.Infrastructure.Entities;
using Finance.Infrastructure.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Finance.Infrastructure.Consumers
{
    public class OrderPaidConsumer : IConsumer<OrderPaidEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderPaidConsumer> _logger;

        public OrderPaidConsumer(IUnitOfWork unitOfWork, ILogger<OrderPaidConsumer> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderPaidEvent> context)
        {
            try
            {
                var message = context.Message;
                var organizerId = message.OrganizerId;

                if (organizerId == Guid.Empty)
                {
                    _logger.LogWarning("Bỏ qua OrderPaidEvent cho OrderId {OrderId} do OrganizerId trống.", message.OrderId);
                    return;
                }

                var wallet = await _unitOfWork.WalletRepository.GetOneAsync<Wallet>(
                    filter: w => w.OrganizerId == organizerId);

                if (wallet == null)
                {
                    wallet = new Wallet(organizerId);
                    await _unitOfWork.WalletRepository.CreateAsync(wallet);
                    await _unitOfWork.SaveChangesAsync();
                }

                var existingTx = await _unitOfWork.WalletTransactionRepository.GetOneUntrackedAsync<WalletTransaction>(
                    filter: t => t.OrderId == message.OrderId && t.Type == WalletTransactionType.Revenue);

                if (existingTx != null)
                {
                    _logger.LogInformation("Giao dịch doanh thu cho OrderId {OrderId} đã tồn tại.", message.OrderId);
                    return;
                }

                var releaseAt = message.ShowtimeEndAt.Date.AddDays(1);

                var transaction = new WalletTransaction(
                    walletId: wallet.Id,
                    orderId: message.OrderId,
                    eventId: message.EventId,
                    categoryId: message.CategoryId,
                    amount: message.TotalPrice,
                    type: WalletTransactionType.Revenue,
                    description: $"Doanh thu từ đơn hàng {message.OrderId} của sự kiện {message.EventTitle}",
                    releaseAt: releaseAt
                );

                await _unitOfWork.WalletTransactionRepository.CreateAsync(transaction);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Đã tạo giao dịch chờ giải ngân cho OrderId {OrderId}, dự kiến giải ngân lúc {ReleaseAt}", message.OrderId, releaseAt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý OrderPaidEvent cho OrderId {OrderId}", context.Message.OrderId);
                throw;
            }
        }
    }
}
