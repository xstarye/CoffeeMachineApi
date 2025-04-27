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

        public DeductStockController(IStockService stockService)
        {
            _stockService = stockService;
        }

        [HttpPost("deduct")]
        public async Task<IActionResult> Deduct([FromBody] Stock dto)
        {
            var success = await _stockService.TryDeductStock(dto.ProductId, dto.Quantity);
            return success ? Ok("Stock updated") : BadRequest("Failed to update stock");
        }
    }
}
