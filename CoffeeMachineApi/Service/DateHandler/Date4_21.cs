namespace CoffeeMachineApi.Service.DateHandler
{
    public class Date4_21 : IDateHandler
    {
        private readonly ILogger<Date4_21> _logger;

        public Date4_21(ILogger<Date4_21> logger)
        {
            _logger = logger;
        }
        int IDateHandler.HandleAsync()
        {
            _logger.LogWarning("Fuck on 4-19");
            return StatusCodes.Status226IMUsed;
        }
    }
}
