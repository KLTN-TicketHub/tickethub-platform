using BuildingBlocks.Contracts.Commands.Order;
using BuildingBlocks.Contracts.Commands.Payment;
using BuildingBlocks.Contracts.Events.Order;
using BuildingBlocks.Contracts.Events.Payment;
using MassTransit;
using Ordering.Infrastructure.Entities;

namespace Ordering.Infrastructure.Sagas
{
    public class OrderBookingStateMachine : MassTransitStateMachine<OrderBookingState>
    {
        public OrderBookingStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => CheckoutStarted, x => x.CorrelateById(c => c.Message.OrderId));
            Event(() => PaymentLinkGenerated, x => x.CorrelateById(c => c.Message.OrderId));
            Event(() => PaymentCompleted, x => x.CorrelateById(c => c.Message.OrderId));
            Event(() => PaymentFailed, x => x.CorrelateById(c => c.Message.OrderId));

            Schedule(() => PaymentExpired, instance => instance.TimeoutTokenId, s =>
            {
                s.Delay = TimeSpan.FromMinutes(10);

                s.Received = r => r.CorrelateById(context => context.Message.OrderId);
            });

            Initially(
                When(CheckoutStarted)
                    .Then(context =>
                    {
                        context.Saga.UserId = context.Message.UserId;
                        context.Saga.ShowtimeId = context.Message.ShowtimeId;
                        context.Saga.TotalPrice = context.Message.TotalPrice;
                        context.Saga.CreatedAt = DateTime.UtcNow;
                    })
                    .Send(new Uri("queue:payment-generate-link"), context => new GeneratePaymentLinkCommand
                    {
                        OrderId = context.Saga.CorrelationId,
                        Amount = context.Saga.TotalPrice,
                        Gateway = context.Message.PaymentMethod,
                        CustomerName = context.Message.CustomerName,
                        CustomerEmail = context.Message.CustomerEmail,
                    })
                    .TransitionTo(Reserved));

            During(Reserved,
                When(PaymentLinkGenerated)
                    .Then(context => context.Saga.PaymentLink = context.Message.PaymentLink)
                    .Schedule(PaymentExpired, context => new PaymentTimeoutEvent
                    {
                        OrderId = context.Saga.CorrelationId
                    }),

                When(PaymentCompleted)
                    .Unschedule(PaymentExpired)
                    .Send(new Uri("queue:ordering-confirm-order"),
                        context => new ConfirmOrderCommand { OrderId = context.Saga.CorrelationId })
                    .TransitionTo(Paid)
                    .Finalize(),

                 When(PaymentFailed)
                    .Unschedule(PaymentExpired)
                    .Send(new Uri("queue:ordering-cancel-order"), context => new CancelOrderCommand
                    {
                        OrderId = context.Saga.CorrelationId,
                        Reason = "Thanh toán không thành công"
                    })
                    .TransitionTo(Failed)
                    .Finalize(),

                 When(PaymentExpired.Received)
                    .Send(new Uri("queue:ordering-cancel-order"), context => new CancelOrderCommand
                    {
                        OrderId = context.Saga.CorrelationId,
                        Reason = "Hết hạn thanh toán 10 phút"
                    })
                    .TransitionTo(Failed)
                    .Finalize()
                );
        }

        public State Reserved { get; private set; }

        public State Paid { get; private set; }

        public State Failed { get; private set; }

        public Event<CheckoutStartedEvent> CheckoutStarted { get; private set; }

        public Event<PaymentLinkGeneratedEvent> PaymentLinkGenerated { get; private set; }

        public Event<PaymentCompletedEvent> PaymentCompleted { get; private set; }

        public Event<PaymentFailedEvent> PaymentFailed { get; private set; }

        public Schedule<OrderBookingState, PaymentTimeoutEvent> PaymentExpired { get; private set; } = default!;
    }
    public class PaymentTimeoutEvent
    {
        public Guid OrderId { get; set; }
    }
}
