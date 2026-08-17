using BuildingBlocks.Domain.Exceptions;
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

        public async Task<bool> LockSeatAsync(Guid showtimeId, Guid seatId, Guid userId, TimeSpan ttl, string status = "Selecting")
        {
            string key = $"seat_lock:{showtimeId}:{seatId}";
            string value = $"{userId}:{status}";

            string luaScript = @"
                local val = redis.call('GET', KEYS[1])
                if not val or string.sub(val, 1, string.len(ARGV[1])) == ARGV[1] then
                    redis.call('SET', KEYS[1], ARGV[2], 'EX', tonumber(ARGV[3]))
                    return 1
                else
                    return 0
                end";

            var result = await _redisDb.ScriptEvaluateAsync(luaScript,
                new RedisKey[] { key },
                new RedisValue[] { userId.ToString(), value, (int)ttl.TotalSeconds });

            return (int)result == 1;
        }

        public async Task<bool> UnlockSeatAsync(Guid showtimeId, Guid seatId, Guid userId, bool force = false)
        {
            string key = $"seat_lock:{showtimeId}:{seatId}";

            string luaScript = @"
                local val = redis.call('GET', KEYS[1])
                if not val then
                    return 1
                elseif string.sub(val, 1, string.len(ARGV[1])) == ARGV[1] then
                    if ARGV[2] == '0' then
                        if string.find(val, ':Checkout', 1, true) then
                            return -1
                        end
                    end
                    return redis.call('DEL', KEYS[1])
                else
                    return 0
                end";

            var result = await _redisDb.ScriptEvaluateAsync(luaScript,
                new RedisKey[] { key },
                new RedisValue[] { userId.ToString(), force ? "1" : "0" });

            if ((int)result == -1)
            {
                throw new BusinessRuleException("Ghế đang trong quá trình thanh toán, không thể hủy.");
            }

            return (int)result == 1;
        }

        public async Task<Dictionary<string, (string Status, string UserId)>> GetLockedSeatsAsync(Guid showtimeId)
        {
            Dictionary<string, (string Status, string UserId)> result = new Dictionary<string, (string Status, string UserId)>();
            EndPoint[] endpoints = _connection.GetEndPoints();
            IServer server = _connection.GetServer(endpoints.First());
            string pattern = $"seat_lock:{showtimeId}:*";
            RedisKey[] keys = server.Keys(database: _redisDb.Database, pattern: pattern).ToArray();

            if (keys.Length == 0)
                return result;

            RedisValue[] values = await _redisDb.StringGetAsync(keys);

            for (int i = 0; i < keys.Length; i++)
            {
                if (values[i].IsNullOrEmpty)
                    continue;

                string seatIdStr = keys[i].ToString().Split(':').Last();
                string[] parts = values[i].ToString().Split(':');
                string userId = parts[0];
                string status = parts.Length > 1 ? parts[1] : "Selecting";
                result[seatIdStr] = (status, userId);
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
            RedisKey[] keys = server.Keys(database: _redisDb.Database, pattern: pattern).ToArray();

            if (keys.Length == 0)
                return total;

            RedisValue[] values = await _redisDb.StringGetAsync(keys);

            foreach (RedisValue value in values)
            {
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
