using Confluent.Kafka;
using System.Text.Json;
using KafkaProducer;
using KafkaProducer.Settings;

class Program
{
    // private static readonly string settingsFile = "../../../../MongoToRedis/configuration.yaml";
    // private static readonly string settingsFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configuration.yaml");
    private static readonly string settingsFile = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName, "configuration.yaml");

    static async Task Main(string[] args)
    {
        // Start();

        var kafkaSettings = ConfigurationLoader.LoadSettings<ProducerSettings>(settingsFile, nameof(ProducerSettings));
        var timeOutSettings = ConfigurationLoader.LoadSettings<TimeOutSettings>(settingsFile, nameof(TimeOutSettings));

        var config = new ProducerConfig { BootstrapServers = kafkaSettings.BootstrapServers };

        using (var producer = new ProducerBuilder<Null, string>(config).Build())
        {
            int reporterId = 1;

            while (true)
            {
                EventMessage eventMessage = new EventMessage()
                {
                    ReporterId = reporterId,
                    Timestamp = DateTime.UtcNow,
                    MetricId = new Random().Next(1, 11),
                    MetricValue = new Random().Next(1, 101),
                    Message = kafkaSettings.Message
                };

                var jsonString = JsonSerializer.Serialize(eventMessage);

                producer.Produce(kafkaSettings.PublishName, new Message<Null, string> { Value = jsonString }, (deliveryReport) =>
                {
                    if (deliveryReport.Error.Code != ErrorCode.NoError)
                    {
                        Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
                    }
                    else
                    {
                        Console.WriteLine($"Sent: {jsonString}");
                    }
                });

                reporterId++;

                await Task.Delay(timeOutSettings.SleepTime);
            }
        }
    }
}