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
        private readonly IServiceScopeFactory _scopeFactory;

        public RedisKeyspaceNotificationHostedService(
            IConnectionMultiplexer connection,
            IHubContext<SeatMapHub> hubContext,
            ILogger<RedisKeyspaceNotificationHostedService> logger,
            IServiceScopeFactory scopeFactory)
        {
            _connection = connection;
            _hubContext = hubContext;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var db = _connection.GetDatabase();
            int dbIndex = db.Database;

            bool keyspaceEventsEnabled = false;
            try
            {
                var endpoints = _connection.GetEndPoints();
                foreach (var endpoint in endpoints)
                {
                    var server = _connection.GetServer(endpoint);
                    if (server.IsReplica) continue;

                    await server.ConfigSetAsync("notify-keyspace-events", "KEgx");

                    var current = await server.ConfigGetAsync("notify-keyspace-events");
                    var currentVal = current.FirstOrDefault().Value.ToString();
                    _logger.LogInformation("notify-keyspace-events is now '{Value}' on Redis server: {Endpoint}", currentVal, endpoint);

                    if (currentVal.Contains('x') || currentVal.Contains('E'))
                    {
                        keyspaceEventsEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not configure notify-keyspace-events automatically. Falling back to polling mode.");
            }

            if (keyspaceEventsEnabled)
            {
                _logger.LogInformation("Redis keyspace expiry events enabled. Subscribing to expired key events.");
                var subscriber = _connection.GetSubscriber();
                string channelName = $"__keyevent@{dbIndex}__:expired";
                var redisChannel = RedisChannel.Literal(channelName);

                await subscriber.SubscribeAsync(redisChannel, async (channel, value) =>
                {
                    try
                    {
                        string expiredKey = value.ToString();
                        await HandleExpiredKey(expiredKey);
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
            else
            {
                _logger.LogWarning("Redis keyspace events unavailable. Using polling fallback every 30s to detect expired seat locks.");
                await RunPollingFallbackAsync(dbIndex, stoppingToken);
            }
        }

        private async Task HandleExpiredKey(string expiredKey)
        {
            if (!expiredKey.StartsWith("seat_lock:")) return;

            var parts = expiredKey.Split(':');
            if (parts.Length != 3) return;

            if (Guid.TryParse(parts[1], out var showtimeId) && Guid.TryParse(parts[2], out var seatId))
            {
                _logger.LogInformation("Seat lock expired. Showtime: {ShowtimeId}, Seat: {SeatId}. Broadcasting Available via SignalR.", showtimeId, seatId);

                await _hubContext.Clients.Group($"showtime_{showtimeId}")
                    .SendAsync("SeatStateChanged", new
                    {
                        seatId = seatId.ToString(),
                        status = "Available"
                    });
            }
        }

        private async Task RunPollingFallbackAsync(int dbIndex, CancellationToken stoppingToken)
        {
            var trackedKeys = new Dictionary<string, DateTime>();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var db = _connection.GetDatabase();
                    var server = _connection.GetServer(_connection.GetEndPoints().First());
                    var currentKeys = new HashSet<string>();

                    await foreach (var key in server.KeysAsync(database: dbIndex, pattern: "seat_lock:*"))
                    {
                        currentKeys.Add(key.ToString());
                        if (!trackedKeys.ContainsKey(key.ToString()))
                        {
                            trackedKeys[key.ToString()] = DateTime.UtcNow;
                        }
                    }

                    var expiredKeys = trackedKeys.Keys.Where(k => !currentKeys.Contains(k)).ToList();
                    foreach (var expiredKey in expiredKeys)
                    {
                        _logger.LogInformation("[Polling] Detected expired seat lock key: {Key}", expiredKey);
                        await HandleExpiredKey(expiredKey);
                        trackedKeys.Remove(expiredKey);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[Polling] Error during seat lock polling.");
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
