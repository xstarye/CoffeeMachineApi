using CoffeeMachineApi.Service.CountHandler;

namespace CoffeeMachineApi.Factory
{
    public interface ICountHandlerFactory
    {
        ICountHandler GetHandler(int count);
    }
}
