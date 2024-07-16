namespace MongoToRedis
{
    public class EventMessage
    {
        public int ReporterId { get; set; }
        public required DateTime Timestamp { get; set; }
        public int MetricId { get; set; }
        public int MetricValue { get; set; }
        public required string Message { get; set; }
    }
}