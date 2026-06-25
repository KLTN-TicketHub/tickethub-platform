using Inventory.API.Hubs;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;

namespace Inventory.API.Services
{
    public class RedisKeyspaceNotificationHostedService : BackgroundService
    {
        private readonly IConnectionMultiplexer _connection;
        private readonly IHubContext<SeatMapHub> _hubContext;
        private readonly ILogger<RedisKeyspaceNotificationHostedService> _logger;

        public RedisKeyspaceNotificationHostedService(
            IConnectionMultiplexer connection,
            IHubContext<SeatMapHub> hubContext,
            ILogger<RedisKeyspaceNotificationHostedService> logger)
        {
            _connection = connection;
            _hubContext = hubContext;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var db = _connection.GetDatabase();
            int dbIndex = db.Database;

            try
            {
                var endpoints = _connection.GetEndPoints();
                foreach (var endpoint in endpoints)
                {
                    var server = _connection.GetServer(endpoint);
                    if (server.IsReplica) continue;

                    await server.ConfigSetAsync("notify-keyspace-events", "Ex");
                    _logger.LogInformation("Successfully configured notify-keyspace-events to 'Ex' on Redis server: {Endpoint}", endpoint);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not configure notify-keyspace-events to 'Ex' automatically on startup. If this is a managed/cloud Redis, please ensure 'notify-keyspace-events' is set to 'Ex' manually on your server configuration.");
            }

            var subscriber = _connection.GetSubscriber();
            string channelName = $"__keyevent@{dbIndex}__:expired";
            var redisChannel = RedisChannel.Literal(channelName);

            _logger.LogInformation("Subscribed to Redis expired events on channel: {Channel}", channelName);

            await subscriber.SubscribeAsync(redisChannel, async (channel, value) =>
            {
                try
                {
                    string expiredKey = value.ToString();

                    if (expiredKey.StartsWith("seat_lock:"))
                    {
                        var parts = expiredKey.Split(':');
                        if (parts.Length == 3)
                        {
                            var showtimeIdStr = parts[1];
                            var seatIdStr = parts[2];

                            if (Guid.TryParse(showtimeIdStr, out var showtimeId) && Guid.TryParse(seatIdStr, out var seatId))
                            {
                                _logger.LogInformation("Seat lock expired on Redis. Showtime: {ShowtimeId}, Seat: {SeatId}. Broad-casting Available state to clients via SignalR...", showtimeId, seatId);

                                await _hubContext.Clients.Group($"showtime_{showtimeId}")
                                    .SendAsync("SeatStateChanged", new
                                    {
                                        seatId = seatId.ToString(),
                                        status = "Available"
                                    });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing Redis keyspace expired event.");
                }
            });

            var tcs = new TaskCompletionSource();
            using (stoppingToken.Register(s => ((TaskCompletionSource)s!).SetResult(), tcs))
            {
                await tcs.Task;
            }

            _logger.LogInformation("Unsubscribing from Redis keyspace events...");
            await subscriber.UnsubscribeAsync(redisChannel);
        }
    }
}
