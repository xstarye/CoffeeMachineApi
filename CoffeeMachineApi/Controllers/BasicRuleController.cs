using CoffeeMachineApi.Service.Cache;
using CoffeeMachineApi.Service.Datetime;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeMachineApi.Controllers
{
    [ApiController]
    //test git
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
            if (handler != null && handler != "")
            {
                return StatusCode(200);
            }
            else
            {
                string time1 = "4_21";
                var handler1 = await _dataCache.GetCachedDataAsync(time1);
                if (handler1 != null && handler1 != "")
                {
                    return StatusCode(200);
                }
            }
            return StatusCode(210);
        }

    }
}
