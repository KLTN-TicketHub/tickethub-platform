using BuildingBlocks.Contracts.Commands.Inventory;
using BuildingBlocks.Contracts.Commands.Order;
using BuildingBlocks.Contracts.Commands.Payment;
using BuildingBlocks.Contracts.Events.Inventory;
using BuildingBlocks.Contracts.Events.Order;
using BuildingBlocks.Contracts.Events.Payment;
using MassTransit;
using Ordering.Infrastructure.Entities;

namespace Ordering.Infrastructure.Sagas
{
    public class OrderRefundStateMachine : MassTransitStateMachine<OrderRefundState>
    {
        public OrderRefundStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => OrderRefundRequested, x => x.CorrelateById(c => c.Message.OrderId));
            Event(() => OrderTicketsInvalidated, x => x.CorrelateById(c => c.Message.OrderId));
            Event(() => PaymentRefunded, x => x.CorrelateById(c => c.Message.OrderId));
            Event(() => PaymentRefundFailed, x => x.CorrelateById(c => c.Message.OrderId));

            Initially(
                When(OrderRefundRequested)
                    .Then(context =>
                    {
                        context.Saga.EventId = context.Message.EventId;
                        context.Saga.CreatedAt = DateTime.UtcNow;
                    })
                    .Send(new Uri("queue:inventory-invalidate-order-tickets"), context => new InvalidateOrderTicketsCommand
                    {
                        OrderId = context.Saga.CorrelationId
                    })
                    .TransitionTo(AwaitingTicketInvalidation));

            During(AwaitingTicketInvalidation,
                When(OrderTicketsInvalidated)
                    .Then(context => context.Saga.RefundableAmount = context.Message.RefundableAmount)
                    .IfElse(context => context.Message.RefundableAmount > 0,
                        thenBinder => thenBinder
                            .Send(new Uri("queue:payment-refund-order"), context => new RefundPaymentCommand
                            {
                                OrderId = context.Saga.CorrelationId,
                                Amount = context.Message.RefundableAmount,
                                Reason = "Sự kiện đã bị hủy"
                            })
                            .TransitionTo(AwaitingPaymentRefund),
                        elseBinder => elseBinder
                            .TransitionTo(NoRefundNeeded)
                            .Finalize()));

            During(AwaitingPaymentRefund,
                When(PaymentRefunded)
                    .Send(new Uri("queue:ordering-finalize-refund"), context => new FinalizeOrderRefundCommand
                    {
                        OrderId = context.Saga.CorrelationId,
                        RefundedAmount = context.Message.RefundedAmount,
                        VnpayRefundTransactionId = context.Message.VnpayRefundTransactionId
                    })
                    .TransitionTo(Refunded)
                    .Finalize(),

                When(PaymentRefundFailed)
                    .Then(context => context.Saga.FailureReason = context.Message.Reason)
                    .TransitionTo(RefundFailed));
        }

        public State AwaitingTicketInvalidation { get; private set; }

        public State AwaitingPaymentRefund { get; private set; }

        public State NoRefundNeeded { get; private set; }

        public State Refunded { get; private set; }

        public State RefundFailed { get; private set; }

        public Event<OrderRefundRequestedEvent> OrderRefundRequested { get; private set; }

        public Event<OrderTicketsInvalidatedEvent> OrderTicketsInvalidated { get; private set; }

        public Event<PaymentRefundedEvent> PaymentRefunded { get; private set; }

        public Event<PaymentRefundFailedEvent> PaymentRefundFailed { get; private set; }
    }
}
