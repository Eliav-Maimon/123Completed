using System.Text.Json;
using Confluent.Kafka;
using KafkaConsumer;
using KafkaConsumer.Settings;
using MongoDB.Driver;

class Program
{
    // private static readonly string settingsFile = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName, "configuration.yaml");
    private static readonly string settingsFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configuration.yaml");
    static void Main(string[] args)
    {
        var consumerSettings = ConfigurationLoader.LoadSettings<ConsumerSettings>(settingsFile, nameof(ConsumerSettings));
        var mongoDBSettings = ConfigurationLoader.LoadSettings<MongoDBSettings>(settingsFile, nameof(MongoDBSettings));

        var config = new ConsumerConfig
        {
            GroupId = consumerSettings.GroupId,
            BootstrapServers = consumerSettings.BootstrapServers,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
        {
            consumer.Subscribe(consumerSettings.SubscribeName);
            var mongoClient = new MongoClient(mongoDBSettings.ConnectionString);
            var database = mongoClient.GetDatabase(mongoDBSettings.DatabaseName);
            var collection = database.GetCollection<EventMessage>(mongoDBSettings.CollectionName);

            try
            {
                while (true)
                {
                    var consumeResult = consumer.Consume();

                    try
                    {
                        var eventMessage = JsonSerializer.Deserialize<EventMessage>(consumeResult.Message.Value);
                        Console.WriteLine("Deserialized message");

                        if (eventMessage != null)
                        {
                            collection.InsertOne(eventMessage);
                            consumer.Commit(consumeResult);
                            Console.WriteLine($"Inserted event into MongoDB: {eventMessage}");
                        }
                    }
                    catch (JsonException jsonEx)
                    {
                        Console.WriteLine($"Invalid JSON format: {consumeResult.Message.Value}");
                        Console.WriteLine($"Error: {jsonEx.Message}");
                    }
                }
            }
            catch (ConsumeException e)
            {
                Console.WriteLine($"Consume error: {e.Error.Reason}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                consumer.Close();
            }
        }
    }
}