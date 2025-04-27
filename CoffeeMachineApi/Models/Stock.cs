namespace CoffeeMachineApi.Models
{
    public class Stock
    {
        public string ProductId { get; set; }

        public int Quantity { get; set; }

        public byte[] RowVersion { get; set; }
    }
}
