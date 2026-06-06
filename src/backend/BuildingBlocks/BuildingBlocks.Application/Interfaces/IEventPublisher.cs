namespace BuildingBlocks.Application.Interfaces
{
    public interface IEventPublisher
    {
        void Publish<TEvent>(TEvent @event)
            where TEvent : class;
    }
}
