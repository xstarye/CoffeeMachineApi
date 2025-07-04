using CoffeeMachineApi.Models;
using CoffeeMachineApi.Service.Stock;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeMachineApi.Controllers
{
    [ApiController]
    [Route("api/stock")]
    public class DeductStockController : ControllerBase
    {
        private readonly IStockService _stockService;
        private readonly IStockServiceWithRedlock _stockServiceWithRedlock;

        public DeductStockController(IStockService stockService, IStockServiceWithRedlock stockServiceWithRedlock)
        {
            _stockService = stockService;
            _stockServiceWithRedlock = stockServiceWithRedlock;
        }

        [HttpPost("deduct")]
        public async Task<IActionResult> Deduct([FromBody] Stock dto)
        {
            var success = await _stockService.TryDeductStock(dto.ProductId, dto.Quantity);
            return success ? Ok("Stock updated") : BadRequest("Failed to update stock");
        }

        [HttpPost("deduct1")]
        public async Task<IActionResult> Deduct1([FromBody] Stock dto)
        {
            var success = await _stockServiceWithRedlock.TryDeductStock(dto.ProductId, dto.Quantity);
            return success ? Ok("Stock updated") : BadRequest("Failed to update stock");
        }
    }
}
