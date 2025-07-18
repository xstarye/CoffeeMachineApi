using Confluent.Kafka;
using System.Text.Json;
using SharedLib;
using Microsoft.AspNetCore.Mvc;
namespace CoffeeMachineApi.Service.Kafka
{
    public class KafkaProducer
    {
        private readonly IProducer<Null, string> _producer;

        public KafkaProducer(IConfiguration config)
        {
            var conf = new ProducerConfig { BootstrapServers = config["Kafka:BootstrapServers"] };
            _producer = new ProducerBuilder<Null, string>(conf).Build();
        }

        public async Task PublishStockChanged([FromBody] StockChange data)
        {
            var msg = JsonSerializer.Serialize(data);
            await _producer.ProduceAsync("stock-events", new Message<Null, string> { Value = msg });
        }
    }
}
