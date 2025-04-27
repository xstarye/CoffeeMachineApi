namespace CoffeeMachineApi.Models
{
    public class WeatherResponse
    {
        public MainInfo? Main { get; set; }
        public class MainInfo { public double Temp { get; set; } }
    }
}
