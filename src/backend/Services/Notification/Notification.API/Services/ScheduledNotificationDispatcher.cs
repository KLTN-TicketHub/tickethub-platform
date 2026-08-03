using Notification.Infrastructure.Interfaces.IServices;

namespace Notification.API.Services
{
    public class ScheduledNotificationDispatcher : BackgroundService
    {
        private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(30);

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ScheduledNotificationDispatcher> _logger;

        public ScheduledNotificationDispatcher(
            IServiceScopeFactory scopeFactory,
            ILogger<ScheduledNotificationDispatcher> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using PeriodicTimer timer = new PeriodicTimer(PollInterval);

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await DispatchDueNotificationsAsync(stoppingToken);
            }
        }

        private async Task DispatchDueNotificationsAsync(CancellationToken cancellationToken)
        {
            try
            {
                await using AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();

                INotificationService notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                int dispatched = await notificationService.DispatchDueScheduledAsync(cancellationToken);

                if (dispatched > 0)
                {
                    _logger.LogInformation("Dispatched {Count} scheduled notification(s).", dispatched);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while dispatching scheduled notifications.");
            }
        }
    }
}
