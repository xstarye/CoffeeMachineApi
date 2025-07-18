namespace CoffeeMachineApi.Service.Stock
{
    public interface IStockServiceWithRedlock
    {
        Task<bool> TryDeductStock(string productId, int qty, int maxRetries = 3);
    }
}
