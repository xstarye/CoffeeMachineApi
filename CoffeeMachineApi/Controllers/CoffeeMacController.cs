using Microsoft.AspNetCore.Mvc;
using CoffeeMachineApi.Models;
using CoffeeMachineApi.Service.Datetime;
using CoffeeMachineApi.Service.CoffeeCounter;
using CoffeeMachineApi.Factory;
using System.Net.NetworkInformation;

namespace CoffeeMachineApi.Controllers
{
    [ApiController]
    [Route("brew-coffee")]
    public class CoffeeMacController : ControllerBase
    {
        private readonly IDatetime _time;
        private readonly ILogger<CoffeeMacController> _logger;
        private readonly IWeather _weather;
        private readonly IDateHandlerFactory _dateFactory;
        private readonly ICountHandlerFactory _countFactory;
        private readonly ICoffeeCountFactory _coffeeCountFactory;
        private readonly CountState _countState;
        public CoffeeMacController(
            IDatetime time,
            ILogger<CoffeeMacController> logger,
            IWeather weather,
            IDateHandlerFactory dateFactory,
            ICountHandlerFactory countFactory,
            ICoffeeCountFactory coffeeCountFactory,
            CountState countState)
        {
            _time = time;
            _logger = logger;
            _weather = weather;
            _dateFactory = dateFactory;
            _countFactory = countFactory;
            _coffeeCountFactory = coffeeCountFactory;
            _countState = countState;
        }

        [HttpGet]
        public async Task<IActionResult> GetCoffee()
        {
            var now = _time.Now;
            string time = now.Month + "_" + now.Day;

            var handler = _dateFactory.GetHandler(time);
            if (handler != null)
            { 
                return StatusCode(handler.HandleAsync());
            }
            /*
            // check if the date is 4-1
            if (now.Month == 4 && now.Day == 1)
            {
                _logger.LogWarning("Teapot mode activated on April 1st");
                return StatusCode(StatusCodes.Status418ImATeapot);
            }
            */
            var handlerC = _coffeeCountFactory.GetHandler(time);
            int count = 0;
            if (handlerC != null)
            {
                count = handlerC.IncrementAndGet(ref _countState.GlobalCount);
            }
            /*
            var count = _counter.IncrementAndGet();
            */
            _logger.LogInformation("Coffee request #{Count} at {Time}", count, now);

            var handlerCh = _countFactory.GetHandler(count);
            if (handlerCh != null)
            {
                return StatusCode(handlerCh.HandleAsync(count));
            }
            /*
            if (count % 5 == 0)
            {
                _logger.LogWarning("Machine is out of coffee on request #{Count}", count);
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }
            */
            //if the temp is >30
            var temp = await _weather.GetTemperature();
            var message = (temp.HasValue && temp.Value > 30)
                ? "Your refreshing iced coffee is ready"
                : "Your piping hot coffee is ready";

            var response = new ResponseDto
            {
                //In actual scenarios, we can get message from the database and configure by backend management web to prevent more return messages in the future.
                Message = message,
                Prepared = now.ToString("yyyy-MM-ddTHH:mm:sszzz")
            };

            return Ok(response);
        }
    }
}
