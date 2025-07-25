using CoffeeMachineApi.Data;
using CoffeeMachineApi.Entity;
using CoffeeMachineApi.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;

namespace CoffeeMachineApi.Service.Cache
{
    public class CoffeeDateCache : ICoffeeDateCache, IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMemoryCache _cache;
        private Timer _timer;
        private CoffeeDbContext _dataContext;

        private readonly RedisLockProvider _lock;
        //private const string CacheKey = "MyDataCache";

        public CoffeeDateCache(IServiceScopeFactory scopeFactory, IMemoryCache cache, CoffeeDbContext dataContext, RedisLockProvider lockProvider)
        {
            _scopeFactory = scopeFactory;
            _cache = cache;
            this._dataContext = dataContext;
            _lock = lockProvider;
        }

        public Task<string> GetCachedDataAsync(string date)
        {
            return Task.FromResult(_cache.Get<string>(date) ?? "");
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //await LoadData();
            //_timer = new Timer(_ => LoadData(), null, TimeSpan.FromHours(1), TimeSpan.FromHours(1));//这里已经有定时了
            //return Task.CompletedTask;
            var timer = new PeriodicTimer(TimeSpan.FromHours(1));
            await Task.Run(async () =>
            {
                do
                {
                    await LoadData();
                } while (await timer.WaitForNextTickAsync(cancellationToken));
            });
        }

        public async Task LoadData()
        {
            var lockKey = $"date_lock";
            var token = Guid.NewGuid().ToString();

            if (!await _lock.LockAsync(lockKey, token, TimeSpan.FromSeconds(30)))
                return;
            try
            {
                using var scope = _scopeFactory.CreateScope();
                //var dbContext = scope.ServiceProvider.GetRequiredService<CoffeeDbContext>();
                //var data = _dataContext.DateRule.ToList();
                var data = await _dataContext.DateRule
                    .AsNoTracking()
                    .Select(x => new { x.Date, x.Message }) // 或自定义 DTO
                    .ToListAsync();
                foreach (var item in data)
                {
                    _cache.Set(item.Date, item.Message, TimeSpan.FromMinutes(60));
                    var options = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(60))
                        .SetSlidingExpiration(TimeSpan.FromMinutes(20)); // 如果缓存项在 20 分钟内被访问过，则延长其过期时间；如果一直没有被访问，则 20 分钟后过期。

                    _cache.Set(item.Date, item.Message, options);
                }
            }
            finally
            {
                await _lock.UnlockAsync(lockKey, token);
            }
            //_cache.Set(CacheKey, data, TimeSpan.FromHours(2));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Dispose();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
