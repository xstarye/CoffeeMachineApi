using CoffeeMachineApi.Service.CoffeeCounter;

namespace CoffeeMachineApi.Factory
{
    public class CoffeeCountFactory : ICoffeeCountFactory
    {
        private readonly Dictionary<string, ICoffeeCounter> _handlers;

        public CoffeeCountFactory(IEnumerable<ICoffeeCounter> handlers)
        {
            _handlers = handlers.ToDictionary(
                h => h.GetType().Name.Replace("Counter", ""),
                h => h,
                StringComparer.OrdinalIgnoreCase);
        }

        public ICoffeeCounter GetHandler(string date)
        {
            if (_handlers.TryGetValue(date, out var handler))
            {
                return handler;
            }
            else if (_handlers.TryGetValue("Coffee", out var handlerD))
            {
                return handlerD;
            }
            return null;
            
            //throw new NotSupportedException($"No handler for type '{type}'");
        }
    }
}
