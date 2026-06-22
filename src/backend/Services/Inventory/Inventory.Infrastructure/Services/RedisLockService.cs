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
            List<RedisKey> keys = server.Keys(database: _redisDb.Database, pattern: pattern).ToList();

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

        public async Task<bool> LockTicketsAsync(Guid showtimeId, Guid ticketTypeId, Guid userId, int quantity, TimeSpan ttl)
        {
            string key = $"ticket_lock:{showtimeId}:{ticketTypeId}:{userId}";
            return await _redisDb.StringSetAsync(key, quantity.ToString(), ttl);
        }

        public async Task<bool> UnlockTicketsAsync(Guid showtimeId, Guid ticketTypeId, Guid userId)
        {
            string key = $"ticket_lock:{showtimeId}:{ticketTypeId}:{userId}";
            await _redisDb.KeyDeleteAsync(key);
            return true;
        }

        public async Task<int> GetLockedTicketsQuantityAsync(Guid showtimeId, Guid ticketTypeId)
        {
            int total = 0;
            EndPoint[] endpoints = _connection.GetEndPoints();
            IServer server = _connection.GetServer(endpoints.First());
            string pattern = $"ticket_lock:{showtimeId}:{ticketTypeId}:*";
            List<RedisKey> keys = server.Keys(database: _redisDb.Database, pattern: pattern).ToList();

            foreach (var key in keys)
            {
                RedisValue value = await _redisDb.StringGetAsync(key);
                if (!value.IsNullOrEmpty && int.TryParse(value.ToString(), out int qty))
                {
                    total += qty;
                }
            }

            return total;
        }

        public async Task<int> GetUserLockedTicketsQuantityAsync(Guid showtimeId, Guid ticketTypeId, Guid userId)
        {
            string key = $"ticket_lock:{showtimeId}:{ticketTypeId}:{userId}";
            RedisValue value = await _redisDb.StringGetAsync(key);
            if (!value.IsNullOrEmpty && int.TryParse(value.ToString(), out int qty))
            {
                return qty;
            }
            return 0;
        }
    }
}
