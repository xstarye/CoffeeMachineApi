using StackExchange.Redis;

namespace CoffeeMachineApi.Utils
{
    //SET key token NX PX 30000, 这个例子就是向单机redis发送这个命令，达到简单的redis加锁
    /*
    SET：设置键值对
    key：锁的键（标识锁的资源）
    token：客户端生成的唯一标识，用于解锁验证
    NX：仅在键不存在时才设置（即加锁）
    PX：设置过期时间，防止死锁（这里通过 expiry 控制）
    */
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
