using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Enums;
using StackExchange.Redis;
using System.Globalization;

namespace Catalog.Infrastructure.Services
{
    public class EventClickTrackingService : IEventClickTrackingService
    {
        private const string UserClickStreamKey = "user-event-clicks-stream";

        private readonly IDatabase _redisDb;
        private readonly IConnectionMultiplexer _connection;

        public EventClickTrackingService(IConnectionMultiplexer connection)
        {
            _connection = connection;
            _redisDb = connection.GetDatabase();
        }

        public async Task RecordClickAsync(Guid eventId, EventClickType clickType, Guid? userId, CancellationToken cancellationToken = default)
        {
            string counterKey = BuildCounterKey(eventId, clickType);
            await _redisDb.StringIncrementAsync(counterKey);

            if (userId.HasValue)
            {
                NameValueEntry[] fields = new NameValueEntry[]
                {
                    new NameValueEntry("eventId", eventId.ToString()),
                    new NameValueEntry("userId", userId.Value.ToString()),
                    new NameValueEntry("clickType", clickType.ToString()),
                    new NameValueEntry("clickedAt", DateTime.UtcNow.ToString("O"))
                };

                await _redisDb.StreamAddAsync(UserClickStreamKey, fields);
            }
        }

        public async Task<List<(Guid EventId, EventClickType ClickType, long Delta)>> GetAndResetCountersAsync(CancellationToken cancellationToken = default)
        {
            List<(Guid EventId, EventClickType ClickType, long Delta)> results = new List<(Guid, EventClickType, long)>();

            IServer server = _connection.GetServer(_connection.GetEndPoints().First());

            await foreach (RedisKey key in server.KeysAsync(database: _redisDb.Database, pattern: "event:*:clicks:*"))
            {
                cancellationToken.ThrowIfCancellationRequested();

                RedisValue value = await _redisDb.StringGetDeleteAsync(key);
                if (value.IsNullOrEmpty) continue;

                string[] parts = key.ToString().Split(':');
                if (parts.Length != 4) continue;
                if (!Guid.TryParse(parts[1], out Guid eventId)) continue;
                if (!Enum.TryParse(parts[3], out EventClickType clickType)) continue;
                if (!long.TryParse(value.ToString(), out long delta)) continue;

                results.Add((eventId, clickType, delta));
            }

            return results;
        }

        public async Task<List<(Guid EventId, Guid UserId, EventClickType ClickType, DateTime ClickedAt)>> DrainUserClicksAsync(CancellationToken cancellationToken = default)
        {
            List<(Guid EventId, Guid UserId, EventClickType ClickType, DateTime ClickedAt)> results = new List<(Guid, Guid, EventClickType, DateTime)>();

            StreamEntry[] entries = await _redisDb.StreamRangeAsync(UserClickStreamKey, "-", "+");
            if (entries.Length == 0) return results;

            foreach (StreamEntry entry in entries)
            {
                Dictionary<string, string> fields = entry.Values.ToDictionary(v => v.Name.ToString(), v => v.Value.ToString());

                if (!fields.TryGetValue("eventId", out string? eventIdStr) || !Guid.TryParse(eventIdStr, out Guid eventId)) continue;
                if (!fields.TryGetValue("userId", out string? userIdStr) || !Guid.TryParse(userIdStr, out Guid userId)) continue;
                if (!fields.TryGetValue("clickType", out string? clickTypeStr) || !Enum.TryParse(clickTypeStr, out EventClickType clickType)) continue;
                if (!fields.TryGetValue("clickedAt", out string? clickedAtStr) || !DateTime.TryParse(clickedAtStr, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime clickedAt)) continue;

                results.Add((eventId, userId, clickType, clickedAt));
            }

            RedisValue[] ids = entries.Select(e => e.Id).ToArray();
            await _redisDb.StreamDeleteAsync(UserClickStreamKey, ids);

            return results;
        }

        private static string BuildCounterKey(Guid eventId, EventClickType clickType)
        {
            return $"event:{eventId}:clicks:{clickType}";
        }
    }
}
