using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace CoffeeMachineApi.Utils
{
    public class RedisDistributedLock
    {
        private readonly IDatabase _redisDb;
        private readonly string _lockKey;
        private readonly string _lockValue;
        private readonly TimeSpan _expiry;

        public RedisDistributedLock(IConnectionMultiplexer redis, string lockKey, TimeSpan expiry)
        {
            _redisDb = redis.GetDatabase();
            _lockKey = lockKey;
            _expiry = expiry;
            _lockValue = Guid.NewGuid().ToString(); // 确保锁唯一性
        }

        public async Task<bool> AcquireAsync()
        {
            // SET NX PX：原子操作设置锁
            return await _redisDb.StringSetAsync(
                key: _lockKey,
                value: _lockValue,
                expiry: _expiry,
                when: When.NotExists);
        }

        public async Task<bool> ReleaseAsync()
        {
            // 释放锁时确保 value 是自己的（防止误删）
            var script = @"
            if redis.call('get', KEYS[1]) == ARGV[1] then
                return redis.call('del', KEYS[1])
            else
                return 0
            end";

            var result = await _redisDb.ScriptEvaluateAsync(
                script,
                new RedisKey[] { _lockKey },
                new RedisValue[] { _lockValue });

            return (int)result == 1;
        }
    }
}
