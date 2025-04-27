using CoffeeMachineApi.Entity;

namespace CoffeeMachineApi.Service.Cache
{
    public interface ICoffeeDateCache
    {
        Task<string> GetCachedDataAsync(string date);
    }
}
