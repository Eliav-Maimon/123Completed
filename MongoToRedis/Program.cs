using MongoDB.Bson;
using MongoDB.Driver;
using MongoToRedis;
using MongoToRedis.Settings;
using StackExchange.Redis;
class Program
{
    private static readonly string settingsFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configuration.yaml");
    // private static readonly string settingsFile = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName, "configuration.yaml");

    private static IMongoCollection<BsonDocument> mongoCollection;

    static async Task Main(string[] args)
    {
        var mongoDBSettings = ConfigurationLoader.LoadSettings<MongoDBSettings>(settingsFile, nameof(MongoDBSettings));
        var redisSettings = ConfigurationLoader.LoadSettings<RedisSettings>(settingsFile, nameof(RedisSettings));
        var timeOutSettings = ConfigurationLoader.LoadSettings<TimeOutSettings>(settingsFile, nameof(TimeOutSettings));
        var timestampSettings = ConfigurationLoader.LoadSettings<TimestampSettings>(settingsFile, nameof(TimestampSettings));

        var mongoClient = new MongoClient(mongoDBSettings.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(mongoDBSettings.DatabaseName);
        mongoCollection = mongoDatabase.GetCollection<BsonDocument>(mongoDBSettings.CollectionName);

        var redis = ConnectionMultiplexer.Connect(redisSettings.ConnectionString);
        var redisDb = redis.GetDatabase();

        while (true)
        {
            DateTime latestDateTime = DateTime.MinValue;
            int sequentialNumber = 0;

            var latestTimestampStr = redisDb.StringGet(timestampSettings.Name).ToString();
            if (!string.IsNullOrEmpty(latestTimestampStr))
            {
                var parts = latestTimestampStr.Split(new char[] { ':' }, 2);
                int.TryParse(parts[0], out sequentialNumber);

                if (DateTime.TryParse(parts[1], out var parsedDate))
                {
                    latestDateTime = parsedDate;
                }
            }

            var filter = Builders<BsonDocument>.Filter.Or(
                Builders<BsonDocument>.Filter.Gt(nameof(EventMessage.Timestamp), latestDateTime),
                Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq(nameof(EventMessage.Timestamp), latestDateTime),
                    Builders<BsonDocument>.Filter.Gt(nameof(EventMessage.ReporterId), sequentialNumber)
                )
            );

            var newDocuments = mongoCollection.Find(filter).Sort(Builders<BsonDocument>.Sort.Ascending(nameof(EventMessage.Timestamp))).ToList();

            var redisEntries = new List<KeyValuePair<RedisKey, RedisValue>>();

            foreach (var doc in newDocuments)
            {
                var timestamp = doc[nameof(EventMessage.Timestamp)];
                var reporterId = doc[nameof(EventMessage.ReporterId)].AsInt32.ToString();
                var key = $"{reporterId}:{timestamp:yyyyMMddHHmmss}";

                var value = doc.ToJson();

                redisEntries.Add(new KeyValuePair<RedisKey, RedisValue>(new RedisKey(key), new RedisValue(value)));
            }

            if (redisEntries.Count > 0)
            {
                var lastDoc = newDocuments.Last();
                var lastTimestamp = lastDoc[nameof(EventMessage.Timestamp)];
                var lastReporterId = lastDoc[nameof(EventMessage.ReporterId)].AsInt32.ToString();
                var lastTimestampStr = $"{lastReporterId}:{lastTimestamp:yyyy-MM-ddTHH:mm:ss}";

                redisEntries.Add(new KeyValuePair<RedisKey, RedisValue>(timestampSettings.Name, lastTimestampStr));

                redisDb.StringSet(redisEntries.ToArray());
            }

            await Task.Delay(timeOutSettings.SleepTime);
        }
    }
}