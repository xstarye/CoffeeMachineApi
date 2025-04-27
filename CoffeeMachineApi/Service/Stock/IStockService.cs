namespace CoffeeMachineApi.Service.Stock
{
    public interface IStockService
    {
        Task<bool> TryDeductStock(string productId, int qty, int maxRetries = 3);
    }
}
