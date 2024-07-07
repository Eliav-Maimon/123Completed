using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KafkaConsumer.Settings
{
    public class MongoDBSettings
    {
        public string ConnectionString { get; init; }
        public string DatabaseName { get; init; }
        public string CollectionName { get; init; }
    }
}