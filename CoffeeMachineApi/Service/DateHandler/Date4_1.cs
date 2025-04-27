
using CoffeeMachineApi.Controllers;
using Confluent.Kafka;
using System.Net.Http;

namespace CoffeeMachineApi.Service.DateHandler
{
    public class Date4_1 : IDateHandler
    {
        private readonly ILogger<Date4_1> _logger;

        public Date4_1(ILogger<Date4_1> logger)
        {
            _logger = logger;
        }
        int IDateHandler.HandleAsync()
        {
            _logger.LogWarning("Teapot mode activated on April 1st");
            return StatusCodes.Status418ImATeapot;
        }
    }
}
