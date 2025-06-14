using StackExchange.Redis;
using System.Diagnostics;

namespace CoffeeMachineApi.Utils
{
    public class RedisRedLock
    {
        private readonly List<IDatabase> _redisDatabases;
        private readonly int _quorum;

        public RedisRedLock(IEnumerable<IConnectionMultiplexer> redisConnections)
        {
            _redisDatabases = redisConnections.Select(c => c.GetDatabase()).ToList();
            _quorum = _redisDatabases.Count / 2 + 1; // 超过半数实例锁定即成功
        }

        public async Task<bool> AcquireAsync(string lockKey, string lockValue, TimeSpan expiry)
        {
            int successCount = 0;
            var stopwatch = Stopwatch.StartNew();
            
            foreach (var db in _redisDatabases)
            {
                try
                {
                    var locked = await db.StringSetAsync(lockKey, lockValue, expiry, When.NotExists);
                    if (locked) successCount++;
                }
                catch
                {
                    // 忽略异常，继续尝试其他节点
                }
            }

            stopwatch.Stop();

            // 检查是否在锁过期前拿到大多数锁
            return successCount >= _quorum && stopwatch.Elapsed < expiry;
        }

        public async Task ReleaseAsync(string lockKey, string lockValue)
        {
            var script = @"
                if redis.call('get', KEYS[1]) == ARGV[1] then
                    return redis.call('del', KEYS[1])
                else
                    return 0
                end";
            
            foreach (var db in _redisDatabases)
            {
                try
                {
                    await db.ScriptEvaluateAsync(script, new RedisKey[] { lockKey }, new RedisValue[] { lockValue });
                }
                catch
                {
                    // 忽略异常
                }
            }
        }
    }
}
