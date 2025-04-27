namespace CoffeeMachineApi.Service.CoffeeCounter
{
    public interface ICoffeeCounter
    {
        //count the number of coffee
        public int IncrementAndGet(ref int count);
        //reset after 5 times
        public void Reset(ref int count);
    }
}
