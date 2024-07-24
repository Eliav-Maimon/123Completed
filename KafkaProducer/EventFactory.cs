using KafkaProducer.Settings;

namespace KafkaProducer;
public class EventFactory
{
    // private static readonly string settingsFile = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName, "configuration.yaml");
    private static readonly string settingsFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configuration.yaml");
    private EventMessageSettings eventSettings;
    private int reporterId;
    public EventFactory()
    {
        eventSettings = ConfigurationLoader.LoadSettings<EventMessageSettings>(settingsFile, nameof(EventMessageSettings));
        if (!int.TryParse(eventSettings.InitialReporterId, out reporterId))
        {
            reporterId = 1;
        }
    }
    public EventMessage CreateEvent()
    {
        EventMessage eventMessage = new EventMessage()
        {
            ReporterId = reporterId,
            Timestamp = DateTime.UtcNow,
            MetricId = new Random().Next(eventSettings.RandomStartMetricId, eventSettings.RandomEndMetricId),
            MetricValue = new Random().Next(eventSettings.RandomStartMetricValue, eventSettings.RandomEndMetricValue),
            Message = eventSettings.Message
        };

        reporterId += eventSettings.ReporterIdIncrement;

        return eventMessage;
    }
}