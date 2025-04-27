using CoffeeMachineApi.Service.CountHandler;
using CoffeeMachineApi.Service.DateHandler;

namespace CoffeeMachineApi.Factory
{
    public interface IDateHandlerFactory
    {
        IDateHandler GetHandler(string type);
    }
}
