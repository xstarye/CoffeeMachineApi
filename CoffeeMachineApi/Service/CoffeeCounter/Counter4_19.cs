namespace CoffeeMachineApi.Service.CoffeeCounter
{
    public class Counter4_19 : ICoffeeCounter
    {
        public int IncrementAndGet(ref int count)
        {
            return Interlocked.Add(ref count,2);
        }

        public void Reset(ref int count) => count = 0;
    }
}
