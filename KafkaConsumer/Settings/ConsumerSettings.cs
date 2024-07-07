namespace KafkaConsumer.Settings;

public class ConsumerSettings
{
    public string GroupId { get; set; }
    public string BootstrapServers { get; set; }
    public string SubscribeName { get; set; }
}