using Inventory.Infrastructure.Interfaces.IServices;
using StackExchange.Redis;
using System.Net;

namespace Inventory.Infrastructure.Services
{
    public class RedisLockService : IRedisLockService
    {
        private readonly IDatabase _redisDb;
        private readonly IConnectionMultiplexer _connection;

        public RedisLockService(IConnectionMultiplexer connection)
        {
            _connection = connection;
            _redisDb = connection.GetDatabase();
        }

        public async Task<bool> LockSeatAsync(Guid showtimeId, Guid seatId, Guid userId, TimeSpan ttl)
        {
            string key = $"seat_lock:{showtimeId}:{seatId}";

            return await _redisDb.StringSetAsync(key, $"{userId}:Selecting", ttl, When.NotExists);
        }
        public async Task<bool> UnlockSeatAsync(Guid showtimeId, Guid seatId, Guid userId)
        {
            string key = $"seat_lock:{showtimeId}:{seatId}";
            RedisValue currentValue = await _redisDb.StringGetAsync(key);

            if (currentValue.IsNullOrEmpty) return true;

            string[] parts = currentValue.ToString().Split(':');

            Guid lockedUser = Guid.Parse(parts[0]);

            if (lockedUser == userId)
            {
                return await _redisDb.KeyDeleteAsync(key);
            }

            return false; 
        }

        public async Task<Dictionary<string, string>> GetLockedSeatsAsync(Guid showtimeId)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            EndPoint[] endpoints = _connection.GetEndPoints();
            IServer server = _connection.GetServer(endpoints.First());
            string pattern = $"seat_lock:{showtimeId}:*";
            List<RedisKey> keys = server.Keys(pattern: pattern).ToList();

            foreach (var key in keys)
            {
                RedisValue value = await _redisDb.StringGetAsync(key);
                if (!value.IsNullOrEmpty)
                {
                    string seatIdStr = key.ToString().Split(':').Last();
                    string[] parts = value.ToString().Split(':');
                    string status = parts.Length > 1 ? parts[1] : "Selecting";
                    result.Add(seatIdStr, status);
                }
            }

            return result;
        }
    }
}
