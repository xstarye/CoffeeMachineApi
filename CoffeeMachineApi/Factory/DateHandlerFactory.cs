using CoffeeMachineApi.Service.DateHandler;

namespace CoffeeMachineApi.Factory
{
    public class DateHandlerFactory : IDateHandlerFactory
    {
        private readonly Dictionary<string, IDateHandler> _handlers;

        public DateHandlerFactory(IEnumerable<IDateHandler> handlers)
        {
            _handlers = handlers.ToDictionary(
                h => h.GetType().Name.Replace("Date", ""),
                h => h,
                StringComparer.OrdinalIgnoreCase);
        }

        public IDateHandler GetHandler(string type)
        {
            if (_handlers.TryGetValue(type, out var handler))
            {
                return handler;
            }
            return null;
            //throw new NotSupportedException($"No handler for type '{type}'");
        }
    }
}
