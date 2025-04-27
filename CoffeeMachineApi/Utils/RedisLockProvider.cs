using StackExchange.Redis;

namespace CoffeeMachineApi.Utils
{
    public class RedisLockProvider
    {
        private readonly IDatabase _redis;

        public RedisLockProvider(IConnectionMultiplexer muxer)
        {
            _redis = muxer.GetDatabase();
        }

        public async Task<bool> LockAsync(string key, string token, TimeSpan expiry) =>
            await _redis.StringSetAsync(key, token, expiry, When.NotExists);

        public async Task UnlockAsync(string key, string token)
        {
            var val = await _redis.StringGetAsync(key);
            if (val == token)
            {
                await _redis.KeyDeleteAsync(key);
            }
        }
    }
}
