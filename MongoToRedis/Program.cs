using MongoDB.Bson;
using MongoDB.Driver;
using MongoToRedis;
using MongoToRedis.Settings;
using StackExchange.Redis;

class Program
{
    private static readonly string settingsFile = "../../../../MongoToRedis/configuration.yaml";
    private static IMongoCollection<BsonDocument> mongoCollection;
    private static IMongoCollection<BsonDocument> timestampUnit;

    static async Task Main(string[] args)
    {
        var mongoDBSettings = ConfigurationLoader.LoadSettings<MongoDBSettings>(settingsFile, nameof(MongoDBSettings));
        var redisSettings = ConfigurationLoader.LoadSettings<RedisSettings>(settingsFile, nameof(RedisSettings));
        var timeOutSettings = ConfigurationLoader.LoadSettings<TimeOutSettings>(settingsFile, nameof(TimeOutSettings));
        var timestampSettings = ConfigurationLoader.LoadSettings<TimestampSettings>(settingsFile, nameof(TimestampSettings));


        var mongoClient = new MongoClient(mongoDBSettings.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(mongoDBSettings.DatabaseName);
        mongoCollection = mongoDatabase.GetCollection<BsonDocument>(mongoDBSettings.CollectionName);
        timestampUnit = mongoDatabase.GetCollection<BsonDocument>(timestampSettings.Name);

        var redis = ConnectionMultiplexer.Connect(redisSettings.ConnectionString);
        var redisDb = redis.GetDatabase();

        while (true)
        {
            DateTime latestDateTime = DateTime.MinValue;
            int sequentialNumber = 0;

            var latestTimestampDoc = await timestampUnit.Find(new BsonDocument()).FirstOrDefaultAsync();
            if (latestTimestampDoc != null)
            {
                var timestampStr = latestTimestampDoc["Timestamp"].AsString;
                var parts = timestampStr.Split(':');
                sequentialNumber = int.Parse(parts[0]);

                if (DateTime.TryParseExact(parts[1], "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None, out var parsedDate))
                {
                    latestDateTime = parsedDate;
                }
            }

            var filter = Builders<BsonDocument>.Filter.Or(
                Builders<BsonDocument>.Filter.Gt("Timestamp", latestDateTime),
                Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("Timestamp", latestDateTime),
                    Builders<BsonDocument>.Filter.Gt("ReporterId", sequentialNumber)
                )
            );

            var newDocuments = mongoCollection.Find(filter).Sort(Builders<BsonDocument>.Sort.Ascending("Timestamp")).ToList();

            foreach (var doc in newDocuments)
            {
                var timestamp = doc["Timestamp"];
                var reporterId = doc["ReporterId"].AsInt32.ToString();
                var key = $"{reporterId}:{timestamp:yyyyMMddHHmmss}";
                var value = doc.ToJson();

                redisDb.StringSet(key, value);

                var newTimestampStr = $"{reporterId}:{timestamp:yyyy-MM-ddTHH:mm:ss}";
                var newTimestampDoc = new BsonDocument { { "Timestamp", newTimestampStr } };
                await timestampUnit.ReplaceOneAsync(new BsonDocument(), newTimestampDoc, new ReplaceOptions { IsUpsert = true });
            }

            await Task.Delay(timeOutSettings.SleepTime);
        }
    }
}