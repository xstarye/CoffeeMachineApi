using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CoffeeMachineApi.Controllers;
using CoffeeMachineApi.Models;
using Moq;
using CoffeeMachineApi.Service.DateHandler;
using System.Linq;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Http;
using CoffeeMachineApi.Service.CountHandler;
using CoffeeMachineApi.Service.Datetime;
using CoffeeMachineApi.Service.CoffeeCounter;
using CoffeeMachineApi.Factory;
using System.Net.NetworkInformation;

namespace CoffieMachineUnitTest
{
    //Unittest, start point from controller
    public class UnitTest
    {
        private readonly Mock<IDatetime> _mockTime;
        private readonly Mock<ILogger<CoffeeMacController>> _mockLogger;
        private readonly Mock<ILogger<Date4_1>> _41Logger;
        private readonly Mock<IWeather> _mockWeather;
        private readonly Mock<IDateHandler> _dateHandler;
        private readonly Mock<ICountHandler> _countHandler;
        private readonly Mock<IDateHandlerFactory> _mockFactory;
        private readonly Mock<ICountHandlerFactory> _countFactory;
        private readonly Mock<ICoffeeCounter> _coffeeCount;
        private readonly Mock<ICoffeeCountFactory> _coffeeCountFactory;
        private readonly Mock<CountState> _countState;

        public UnitTest()
        {
            _mockTime = new Mock<IDatetime>();
            _mockLogger = new Mock<ILogger<CoffeeMacController>>();
            _41Logger = new Mock<ILogger<Date4_1>>();
            _mockWeather = new Mock<IWeather>();
            
            _dateHandler = new Mock<IDateHandler>();
            _mockFactory = new Mock<IDateHandlerFactory>();

            _countHandler = new Mock<ICountHandler>();
            _countFactory = new Mock<ICountHandlerFactory>();

            _coffeeCount = new Mock<ICoffeeCounter>();
            _coffeeCountFactory = new Mock<ICoffeeCountFactory>();
            _countState = new Mock<CountState>();
        }
        /*
        [Fact]
        public async Task Returns_Teapot_On_4_1()
        {
            var mockHandler = new Mock<IDateHandler>();
            //_mockFactory.Setup(f => f.GetHandler("4_1")).Returns(mockHandler.Object);
            // Arrange the date on 4-1, will return the 418
            //_mockFactory.Setup(f => f.GetHandler("4_1")).Returns(_dateHandler.Object);
            mockHandler.Setup(h => h.HandleAsync()).Returns(418);
            var handlers = new Dictionary< string, IDateHandler>  
            {
                { "4_1",_dateHandler.Object }
            };
            var mockFactory = new DateHandlerFactory(handlers);
            //_mockFactory.Setup(f => f.GetHandler("4_1")).Returns(mockHandler.Object);

            //_mockFactory.Setup(f => f._handlers).Returns(_dateHandler.Object);
            //_mockFactory.Setup(f => f.GetHandler("4_1")).Returns(_dateHandler.Object);
            _mockTime.Setup(t => t.Now).Returns(new DateTimeOffset(2025, 4, 1, 9, 0, 0, TimeSpan.Zero));
            var controller = new CoffeeMacController(_mockCounter.Object, _mockTime.Object, _mockLogger.Object, _mockWeather.Object, mockFactory);

            var result = await controller.GetCoffee();

            var status = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(418, status.StatusCode);
        }
        */

        [Fact]
        public async Task Returns_Teapot_On_4_1_New()
        {
            _dateHandler.Setup(h => h.HandleAsync()).Returns(StatusCodes.Status418ImATeapot);

            _mockFactory.Setup(f => f.GetHandler("4_1")).Returns(_dateHandler.Object);

            _mockTime.Setup(t => t.Now).Returns(new DateTimeOffset(2025, 4, 1, 9, 0, 0, TimeSpan.Zero));
            var controller = new CoffeeMacController(_mockTime.Object, _mockLogger.Object, _mockWeather.Object, _mockFactory.Object, _countFactory.Object, _coffeeCountFactory.Object, _countState.Object);

            // Act
            var result = await controller.GetCoffee();

            // Assert
            var status = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(418, status.StatusCode);
        }


        [Fact]
        public async Task Returns_503_5th_Call()
        {
            // Arrange the 5th time, which will return error 503
            _mockTime.Setup(t => t.Now).Returns(DateTimeOffset.UtcNow);

            _countHandler.Setup(h => h.HandleAsync(5)).Returns(StatusCodes.Status503ServiceUnavailable);

            _countFactory.Setup(f => f.GetHandler(5)).Returns(_countHandler.Object);

            var mockAppState = new CountState
            {
                GlobalCount = 4 // 你想要 mock 的值
            };

            _coffeeCount.Setup(h => h.IncrementAndGet(ref mockAppState.GlobalCount)).Returns(5);

            _coffeeCountFactory.Setup(f => f.GetHandler("4_19")).Returns(_coffeeCount.Object);

            var controller = new CoffeeMacController(_mockTime.Object, _mockLogger.Object, _mockWeather.Object, _mockFactory.Object, _countFactory.Object, _coffeeCountFactory.Object, mockAppState);

            var result = await controller.GetCoffee();

            var status = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(503, status.StatusCode);
        }

        /*
        [Fact]
        public async Task Returns_200_Valid_Normal()
        {
            // Arrange the normal situation, set the date to 10-2
            var now = DateTimeOffset.UtcNow;
            _mockTime.Setup(t => t.Now).Returns(now);
            _mockCounter.Setup(c => c.IncrementAndGet()).Returns(1); 

            var controller = new CoffeeMacController(_mockCounter.Object, _mockTime.Object, _mockLogger.Object, _mockWeather.Object, _mockFactory.Object);

            var result = await controller.GetCoffee();

            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic response = okResult.Value!;
            Assert.Equal("Your piping hot coffee is ready", (string)response.Message);
            Assert.Equal(now.ToString("yyyy-MM-ddTHH:mm:sszzz"), (string)response.Prepared);
        }

        [Fact]
        public async Task Response_Format_ISO8601()
        {
            // Arrange to test the format of the time to be in ISO 8601, as set hour span to 9
            var now = new DateTimeOffset(2025, 12, 25, 8, 0, 0, TimeSpan.FromHours(9));
            _mockTime.Setup(t => t.Now).Returns(now);
            _mockCounter.Setup(c => c.IncrementAndGet()).Returns(2);

            var controller = new CoffeeMacController(_mockCounter.Object, _mockTime.Object, _mockLogger.Object, _mockWeather.Object, _mockFactory.Object);

            var result = await controller.GetCoffee();

            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic response = okResult.Value!;
            string prepared = response.Prepared;

            Assert.Matches(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\+\d{2}:\d{2}$", prepared);
        }

        [Fact]
        public async Task Returns_Iced_Coffee_Above_30()
        {
            //arrange to set the temperature to be more than 30
            var now = DateTimeOffset.UtcNow;
            _mockTime.Setup(t => t.Now).Returns(now);
            _mockCounter.Setup(c => c.IncrementAndGet()).Returns(1);
            _mockWeather.Setup(w => w.GetTemperature()).ReturnsAsync(31);

            var controller = new CoffeeMacController(_mockCounter.Object, _mockTime.Object, _mockLogger.Object, _mockWeather.Object, _mockFactory.Object);
            var result = await controller.GetCoffee();

            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic response = okResult.Value!;
            Assert.Equal("Your refreshing iced coffee is ready", (string)response.Message);
        }
        */
    }
}