using BuildingBlocks.Contracts.Commands.Inventory;
using BuildingBlocks.Contracts.Events.Inventory;
using Inventory.Infrastructure.Entities;
using Inventory.Infrastructure.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure.Consumers
{
    public class InvalidateOrderTicketsConsumer : IConsumer<InvalidateOrderTicketsCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InvalidateOrderTicketsConsumer> _logger;

        public InvalidateOrderTicketsConsumer(IUnitOfWork unitOfWork, ILogger<InvalidateOrderTicketsConsumer> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<InvalidateOrderTicketsCommand> context)
        {
            var message = context.Message;
            _logger.LogInformation("Processing InvalidateOrderTicketsCommand for OrderId={OrderId}", message.OrderId);

            try
            {
                IEnumerable<IssuedTicket> tickets = await _unitOfWork.IssuedTicketRepository.GetAllAsync<IssuedTicket>(
                    filter: t => t.OrderId == message.OrderId,
                    cancellation: context.CancellationToken);

                List<IssuedTicket> ticketList = tickets.ToList();
                List<IssuedTicket> validTickets = ticketList.Where(t => t.Status == IssuedTicketStatus.Valid).ToList();
                decimal refundableAmount = validTickets.Sum(t => t.Price);
                int keptCount = ticketList.Count(t => t.Status == IssuedTicketStatus.Used);

                foreach (IssuedTicket ticket in validTickets)
                {
                    ticket.Cancel();
                }

                await _unitOfWork.IssuedTicketRepository.UpdateRangeAsync(validTickets, context.CancellationToken);

                await context.Publish(new OrderTicketsInvalidatedEvent
                {
                    OrderId = message.OrderId,
                    RefundableAmount = refundableAmount,
                    CancelledTicketCount = validTickets.Count,
                    KeptTicketCount = keptCount
                });

                _logger.LogInformation(
                    "Cancelled {CancelledCount} ticket(s), kept {KeptCount} checked-in ticket(s) for OrderId={OrderId}, refundable amount={RefundableAmount}",
                    validTickets.Count, keptCount, message.OrderId, refundableAmount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing InvalidateOrderTicketsCommand for OrderId={OrderId}", message.OrderId);
                throw;
            }
        }
    }
}
