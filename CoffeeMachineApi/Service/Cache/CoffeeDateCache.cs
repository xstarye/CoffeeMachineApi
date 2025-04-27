using CoffeeMachineApi.Data;
using CoffeeMachineApi.Entity;
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
        //private const string CacheKey = "MyDataCache";

        public CoffeeDateCache(IServiceScopeFactory scopeFactory, IMemoryCache cache, CoffeeDbContext dataContext)
        {
            _scopeFactory = scopeFactory;
            _cache = cache;
            this._dataContext = dataContext;
        }

        public Task<string> GetCachedDataAsync(string date)
        {
            return Task.FromResult(_cache.Get<string>(date) ?? "");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            LoadData();
            _timer = new Timer(_ => LoadData(), null, TimeSpan.FromHours(1), TimeSpan.FromHours(1));
            return Task.CompletedTask;
        }

        private void LoadData()
        {
            using var scope = _scopeFactory.CreateScope();
            //var dbContext = scope.ServiceProvider.GetRequiredService<CoffeeDbContext>();
            var data = _dataContext.DateRule.ToList();
            foreach (var item in data)
            {
                _cache.Set(item.Date, item.Message, TimeSpan.FromMinutes(60));
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
