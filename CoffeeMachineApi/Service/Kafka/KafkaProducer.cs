using Confluent.Kafka;
using System.Text.Json;

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

        public async Task PublishStockChanged(string productId, int change)
        {
            var msg = JsonSerializer.Serialize(new { productId, change });
            await _producer.ProduceAsync("stock-events", new Message<Null, string> { Value = msg });
        }
    }
}
