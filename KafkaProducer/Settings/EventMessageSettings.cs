namespace KafkaProducer.Settings;
public class EventMessageSettings
{
    public string Message { get; set; }
    public string InitialReporterId { get; set; }
    public int ReporterIdIncrement { get; set; }
    public int RandomStartMetricId { get; set; }
    public int RandomEndMetricId { get; set; }
    public int RandomStartMetricValue { get; set; }
    public int RandomEndMetricValue { get; set; }
}