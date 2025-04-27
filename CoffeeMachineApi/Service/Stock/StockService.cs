using CoffeeMachineApi.Data;
using CoffeeMachineApi.Service.Kafka;
using CoffeeMachineApi.Utils;
using Microsoft.EntityFrameworkCore;

namespace CoffeeMachineApi.Service.Stock
{
    public class StockService : IStockService
    {
        //private readonly InventoryDbContext _db;
        private readonly RedisLockProvider _lock;
        //private readonly KafkaProducer _kafka;

        //public StockService(InventoryDbContext db, RedisLockProvider lockProvider, KafkaProducer kafka)
        public StockService(RedisLockProvider lockProvider)
        {
            //_db = db;
            _lock = lockProvider;
            //_kafka = kafka;
        }

        public async Task<bool> TryDeductStock(string productId, int qty, int maxRetries = 3)
        {
            var lockKey = $"stock_lock:{productId}";
            var token = Guid.NewGuid().ToString();

            if (!await _lock.LockAsync(lockKey, token, TimeSpan.FromSeconds(30)))
                return false;

            try
            {
                int attempt = 0;

                while (attempt < maxRetries)
                {
                    attempt++;

                    try
                    {
                        /*
                        var stock = await _db.stocks.AsTracking()
                            .FirstOrDefaultAsync(s => s.ProductId == productId);

                        if (stock == null || stock.Quantity < qty)
                            return false;

                        stock.Quantity -= qty;

                        await _db.SaveChangesAsync(); // 乐观锁检查点

                        await _kafka.PublishStockChanged(productId, -qty);
                        */
                        return true;
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (attempt >= maxRetries)
                        {
                            Console.WriteLine($"[乐观锁] 库存更新冲突已达最大重试次数: {maxRetries}");
                            return false;
                        }

                        await Task.Delay(50 * attempt); // 指数退避策略
                    }
                }

                return false;
            }
            finally
            {
                await _lock.UnlockAsync(lockKey, token);
            }
        }
    }
}
