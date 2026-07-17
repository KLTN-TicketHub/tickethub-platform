using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;

namespace Catalog.API.Services
{
    public class EventClickFlushHostedService : BackgroundService
    {
        private static readonly TimeSpan FlushInterval = TimeSpan.FromMinutes(5);

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<EventClickFlushHostedService> _logger;

        public EventClickFlushHostedService(IServiceScopeFactory scopeFactory, ILogger<EventClickFlushHostedService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await FlushAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while flushing event click data to the database.");
                }

                await Task.Delay(FlushInterval, stoppingToken);
            }
        }

        private async Task FlushAsync(CancellationToken cancellationToken)
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            IEventClickTrackingService clickTrackingService = scope.ServiceProvider.GetRequiredService<IEventClickTrackingService>();
            IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            await FlushCountersAsync(clickTrackingService, unitOfWork, cancellationToken);
            await FlushUserClicksAsync(clickTrackingService, unitOfWork, cancellationToken);
        }

        private async Task FlushCountersAsync(IEventClickTrackingService clickTrackingService, IUnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            List<(Guid EventId, Domain.Enums.EventClickType ClickType, long Delta)> counters = await clickTrackingService.GetAndResetCountersAsync(cancellationToken);
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

            foreach (var (eventId, clickType, delta) in counters)
            {
                try
                {
                    EventClickStat? existing = await unitOfWork.EventClickStatRepository.GetOneAsync<EventClickStat>(
                        filter: s => s.EventId == eventId && s.StatDate == today && s.ClickType == clickType,
                        cancellation: cancellationToken);

                    if (existing == null)
                    {
                        await unitOfWork.EventClickStatRepository.CreateAsync(
                            new EventClickStat(eventId, today, clickType, delta), cancellationToken);
                    }
                    else
                    {
                        existing.ClickCount += delta;
                        await unitOfWork.EventClickStatRepository.UpdateAsync(existing, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error writing EventClickStat for EventId {EventId}, ClickType {ClickType}.", eventId, clickType);
                }
            }
        }

        private async Task FlushUserClicksAsync(IEventClickTrackingService clickTrackingService, IUnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            List<(Guid EventId, Guid UserId, Domain.Enums.EventClickType ClickType, DateTime ClickedAt)> userClicks = await clickTrackingService.DrainUserClicksAsync(cancellationToken);
            if (userClicks.Count == 0) return;

            try
            {
                IEnumerable<UserEventClick> entities = userClicks.Select(c => new UserEventClick(c.EventId, c.UserId, c.ClickType, c.ClickedAt));
                await unitOfWork.UserEventClickRepository.CreateRangeAsync(entities, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error bulk-writing {Count} UserEventClick record(s).", userClicks.Count);
            }
        }
    }
}
