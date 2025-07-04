//这里是另外一个方式，实现缓存的定时刷新，在原先的类中已经实现了定时。
//用这个方法，需要在program.cs中builder.Services.AddHostedService<ProductCacheRefreshService>();

namespace CoffeeMachineApi.Service.Cache
{
    public class CoffeeCacheRefreshService : BackgroundService
    {
        private readonly ICoffeeDateCache _coffeeDateCache;
        private readonly ILogger<CoffeeCacheRefreshService> _logger;

        public CoffeeCacheRefreshService(ICoffeeDateCache cacheService, ILogger<CoffeeCacheRefreshService> logger)
        {
            _coffeeDateCache = cacheService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // 启动时先加载一次
            await _coffeeDateCache.LoadData();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                    await _coffeeDateCache.LoadData();
                    _logger.LogInformation("Product cache refreshed at {time}", DateTimeOffset.Now);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error refreshing product cache.");
                }
            }
        }
    }
}
