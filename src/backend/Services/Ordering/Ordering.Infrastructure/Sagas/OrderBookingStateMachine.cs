using BuildingBlocks.Contracts.Events.Order;
using MassTransit;
using Ordering.Infrastructure.Entities;

namespace Ordering.Infrastructure.Sagas
{
    public class OrderBookingStateMachine : MassTransitStateMachine<OrderBookingState>
    {
        public OrderBookingStateMachine()
        {
            InstanceState(x => x.CurrentState);

        }
    }
}
