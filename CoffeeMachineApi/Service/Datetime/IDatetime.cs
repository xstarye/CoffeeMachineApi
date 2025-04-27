namespace CoffeeMachineApi.Service.Datetime
{
    //using interface is for Unit test to set the date
    public interface IDatetime
    {
        DateTimeOffset Now { get; }
    }
}

