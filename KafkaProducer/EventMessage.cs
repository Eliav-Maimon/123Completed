using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KafkaProducer
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