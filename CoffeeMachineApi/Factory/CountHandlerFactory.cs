using CoffeeMachineApi.Service.CountHandler;

namespace CoffeeMachineApi.Factory
{
    public class CountHandlerFactory : ICountHandlerFactory
    {
        private readonly Dictionary<string, ICountHandler> _handlers;

        public CountHandlerFactory(IEnumerable<ICountHandler> handlers)
        {
            _handlers = handlers.ToDictionary(
                h => h.GetType().Name.Replace("Count", ""),
                h => h,
                StringComparer.OrdinalIgnoreCase);
        }

        public ICountHandler GetHandler(int count)
        {
            if (_handlers.TryGetValue(count.ToString(), out var handler))
            {
                return handler;
            }

            int[] multiples = new[] {3, 5 }; // 可以替换成你需要的数字

            foreach (var multiple in multiples)
            {
                if (count % multiple == 0)
                {
                    var key = multiple.ToString();
                    if (_handlers.TryGetValue(key, out handler))
                    {
                        return handler;
                    }
                }
            }
            return null;
            //throw new NotSupportedException($"No handler for type '{type}'");
        }
    }
}
