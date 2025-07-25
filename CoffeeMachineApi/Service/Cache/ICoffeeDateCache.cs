using CoffeeMachineApi.Entity;

namespace CoffeeMachineApi.Service.Cache
{
    public interface ICoffeeDateCache
    {
        Task LoadData();
        Task<string> GetCachedDataAsync(string date);
    }
}
