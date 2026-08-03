using BuildingBlocks.Contracts.Events.Inventory;
using BuildingBlocks.Contracts.Events.Notification;
using BuildingBlocks.Contracts.Events.Order;
using Inventory.Infrastructure.Entities;
using Inventory.Infrastructure.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using QRCoder;

namespace Inventory.Infrastructure.Consumers
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
            var message = context.Message;
            _logger.LogInformation("Processing OrderPaidEvent for OrderId={OrderId}, generating tickets...", message.OrderId);

            try
            {
                var existing = await _unitOfWork.IssuedTicketRepository.GetOneUntrackedAsync<IssuedTicket>(
                    filter: t => t.OrderId == message.OrderId,
                    cancellation: context.CancellationToken);

                if (existing != null)
                {
                    _logger.LogInformation("Tickets already issued for OrderId={OrderId}, skipping.", message.OrderId);
                    return;
                }

                var issuedTicketDtos = new List<IssuedTicketDto>();

                foreach (var item in message.Items)
                {
                    int count = item.SeatId.HasValue ? 1 : item.Quantity;

                    for (int i = 0; i < count; i++)
                    {
                        string qrToken = Guid.NewGuid().ToString("N");
                        string qrBase64 = GenerateQrBase64(qrToken);

                        var issuedTicket = new IssuedTicket
                        {
                            OrderId = message.OrderId,
                            ShowTimeId = message.ShowTimeId,
                            EventId = message.EventId,
                            EventTitle = message.EventTitle,
                            OrganizerName = message.OrganizerName,
                            EventImage = message.EventImage,
                            UserId = message.UserId,
                            CustomerName = message.CustomerName,
                            CustomerEmail = message.CustomerEmail,
                            SeatId = item.SeatId,
                            SeatName = item.SeatName,
                            RowName = item.RowName,
                            TicketTypeId = item.TicketTypeId,
                            TicketTypeName = item.TicketTypeName,
                            Price = item.Price,
                            QrCodeToken = qrToken,
                            QrCodeBase64 = qrBase64,
                            ShowtimeStartAt = message.ShowtimeStartAt,
                            Status = IssuedTicketStatus.Valid,
                            CreatedAt = DateTime.UtcNow
                        };

                        await _unitOfWork.IssuedTicketRepository.CreateAsync(issuedTicket);

                        issuedTicketDtos.Add(new IssuedTicketDto
                        {
                            IssuedTicketId = issuedTicket.Id,
                            QrCodeToken = qrToken,
                            QrCodeBase64 = qrBase64,
                            SeatName = item.SeatName,
                            RowName = item.RowName,
                            TicketTypeName = item.TicketTypeName,
                            Price = item.Price
                        });
                    }
                }

                await _unitOfWork.SaveChangesAsync(context.CancellationToken);

                await context.Publish(new TicketsIssuedEvent
                {
                    OrderId = message.OrderId,
                    ShowTimeId = message.ShowTimeId,
                    EventId = message.EventId,
                    EventTitle = message.EventTitle,
                    ShowtimeStartAt = message.ShowtimeStartAt,
                    CustomerName = message.CustomerName,
                    CustomerEmail = message.CustomerEmail,
                    Tickets = issuedTicketDtos
                });

                await context.Publish(new NotificationRequestedEvent
                {
                    RecipientUserId = message.UserId,
                    Type = "TicketsIssued",
                    Title = "Vé điện tử đã sẵn sàng",
                    Message = $"{issuedTicketDtos.Count} vé cho sự kiện \"{message.EventTitle}\" đã được phát hành. Xuất trình mã QR để check-in tại sự kiện.",
                    LinkUrl = "/my-tickets",
                    ReferenceId = message.OrderId
                });

                _logger.LogInformation("Issued {Count} ticket(s) and published TicketsIssuedEvent for OrderId={OrderId}",
                    issuedTicketDtos.Count, message.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating tickets for OrderId={OrderId}", message.OrderId);
                throw;
            }
        }

        private static string GenerateQrBase64(string qrToken)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(qrToken, QRCodeGenerator.ECCLevel.M);
            using var qrCode = new PngByteQRCode(qrData);
            byte[] pngBytes = qrCode.GetGraphic(10);
            return Convert.ToBase64String(pngBytes);
        }
    }
}
