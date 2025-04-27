using CoffeeMachineApi.Service.DateHandler;

namespace CoffeeMachineApi.Service.CountHandler
{
    public class Count5 : ICountHandler
    {
        private readonly ILogger<Count5> _logger;

        public Count5(ILogger<Count5> logger)
        {
            _logger = logger;
        }
        int ICountHandler.HandleAsync(int count)
        {
            _logger.LogWarning("Machine is out of coffee on request #{Count}", count);
            return StatusCodes.Status503ServiceUnavailable;
        }
    }
}
