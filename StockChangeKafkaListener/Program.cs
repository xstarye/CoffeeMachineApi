using Confluent.Kafka;
using System.Text.Json;
using SharedLib;
var config = new ConsumerConfig
{
    BootstrapServers = "localhost:9092",
    GroupId = "consumer-group-1",
    AutoOffsetReset = AutoOffsetReset.Earliest
};

using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
consumer.Subscribe("stock-events");

Console.WriteLine("Listening for messages...");

try
{
    while (true)
    {
        var consumeResult = consumer.Consume();

        var data = JsonSerializer.Deserialize<StockChange>(consumeResult.Message.Value);
        Console.WriteLine($"Received: {data.productId} - {data.change}");

        // 你可以在这里写入数据库、调用 API、处理业务逻辑等
    }
}
catch (OperationCanceledException)
{
    consumer.Close();
}

