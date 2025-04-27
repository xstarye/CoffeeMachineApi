namespace CoffeeMachineApi.Service.Datetime
{
    public class TimePro : IDatetime
    {
        //set default to now
        public DateTimeOffset Now => DateTimeOffset.Now;
    }
}
