using System.Threading;
namespace CoffeeMachineApi.Service.CoffeeCounter
{
    public class CoffeeCounter : ICoffeeCounter
    {
        //private int _count = 0;

        public int IncrementAndGet(ref int count)
        {
            return Interlocked.Increment(ref count);
        }

        public void Reset(ref int count) => count = 0;
    }
}
