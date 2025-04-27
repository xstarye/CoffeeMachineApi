using CoffeeMachineApi.Service.CoffeeCounter;

namespace CoffeeMachineApi.Factory
{
    public interface ICoffeeCountFactory
    {
        ICoffeeCounter GetHandler(string date);
    }
}
