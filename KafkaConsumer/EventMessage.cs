namespace KafkaConsumer
{
    public class EventMessage
    {
        public int ReporterId { get; set; }
        public DateTime Timestamp { get; set; }
        public int MetricId { get; set; }
        public int MetricValue { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return $"ReporterId: {ReporterId}, Timestamp: {Timestamp}, MetricId: {MetricId}, MetricValue: {MetricValue}, Message: {Message}";
        }
    }
}