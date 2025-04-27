using CoffeeMachineApi.Service.Cache;
using CoffeeMachineApi.Service.Datetime;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeMachineApi.Controllers
{
    [ApiController]
    //[Route("check-coffee")]
    [Route("check-coffee/[controller]/[action]")]
    public class BasicRuleController : ControllerBase
    {
        private readonly ICoffeeDateCache _dataCache;
        private readonly IDatetime _time;

        public BasicRuleController(IDatetime time, ICoffeeDateCache dataCache)
        {
            _time = time;
            _dataCache = dataCache;
        }

        [HttpGet]
        public async Task<IActionResult> CheckSomething()
        {
            var now = _time.Now;
            string time = now.Month + "_" + now.Day;

            var handler = await _dataCache.GetCachedDataAsync(time);
            if (handler != null)
            {
                return StatusCode(200);
            }
            return StatusCode(210);
        }

    }
}
