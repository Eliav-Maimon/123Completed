// using MongoDB.Bson;
// using MongoDB.Driver;
// using MongoToRedis;
// using MongoToRedis.Settings;
// using StackExchange.Redis;

// class Program
// {
//     // private static readonly string settingsFile = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName, "configuration.yaml");
//     private static readonly string settingsFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configuration.yaml");
//     private static IMongoCollection<BsonDocument> mongoCollection;
//     private static IMongoCollection<BsonDocument> timestampUnit;

//     static async Task Main(string[] args)
//     {
//         var mongoDBSettings = ConfigurationLoader.LoadSettings<MongoDBSettings>(settingsFile, nameof(MongoDBSettings));
//         var redisSettings = ConfigurationLoader.LoadSettings<RedisSettings>(settingsFile, nameof(RedisSettings));
//         var timeOutSettings = ConfigurationLoader.LoadSettings<TimeOutSettings>(settingsFile, nameof(TimeOutSettings));
//         var timestampSettings = ConfigurationLoader.LoadSettings<TimestampSettings>(settingsFile, nameof(TimestampSettings));


//         var mongoClient = new MongoClient(mongoDBSettings.ConnectionString);
//         var mongoDatabase = mongoClient.GetDatabase(mongoDBSettings.DatabaseName);
//         mongoCollection = mongoDatabase.GetCollection<BsonDocument>(mongoDBSettings.CollectionName);
//         timestampUnit = mongoDatabase.GetCollection<BsonDocument>(timestampSettings.Name);

//         var redis = ConnectionMultiplexer.Connect(redisSettings.ConnectionString);
//         var redisDb = redis.GetDatabase();

//         while (true)
//         {
//             DateTime latestDateTime = DateTime.MinValue;
//             int sequentialNumber = 0;

//             var latestTimestampDoc = await timestampUnit.Find(new BsonDocument()).FirstOrDefaultAsync();
//             if (latestTimestampDoc != null)
//             {
//                 if (latestTimestampDoc.TryGetValue(nameof(EventMessage.Timestamp), out BsonValue timestampBsonValue))
//                 {
//                     if (timestampBsonValue.BsonType == BsonType.String)
//                     {
//                         var timestampStr = timestampBsonValue.AsString;
//                         var parts = timestampStr.Split(new char[] { ':' }, 2);
//                         sequentialNumber = int.Parse(parts[0]);

//                         if (DateTime.TryParse(parts[1], out var parsedDate))
//                         {
//                             latestDateTime = parsedDate;
//                         }
//                     }
//                     else if (timestampBsonValue.BsonType == BsonType.DateTime)
//                     {
//                         latestDateTime = timestampBsonValue.ToUniversalTime();
//                     }
//                     else
//                     {
//                         throw new FormatException("Unsupported BsonType for Timestamp field.");
//                     }
//                 }
//             }

//             var filter = Builders<BsonDocument>.Filter.Or(
//                 Builders<BsonDocument>.Filter.Gt(nameof(EventMessage.Timestamp), latestDateTime),
//                 Builders<BsonDocument>.Filter.And(
//                     Builders<BsonDocument>.Filter.Eq(nameof(EventMessage.Timestamp), latestDateTime),
//                     Builders<BsonDocument>.Filter.Gt(nameof(EventMessage.ReporterId), sequentialNumber)
//                 )
//             );

//             var newDocuments = mongoCollection.Find(filter).Sort(Builders<BsonDocument>.Sort.Ascending(nameof(EventMessage.Timestamp))).ToList();

//             foreach (var doc in newDocuments)
//             {
//                 var timestamp = doc[nameof(EventMessage.Timestamp)];
//                 var reporterId = doc[nameof(EventMessage.ReporterId)].AsInt32.ToString();
//                 var key = $"{reporterId}:{timestamp:yyyyMMddHHmmss}";

//                 var value = doc.ToJson();

//                 redisDb.StringSet(key, value);

//                 var newTimestampStr = $"{reporterId}:{timestamp:yyyy-MM-ddTHH:mm:ss}";

//                 var newTimestampDoc = new BsonDocument { { nameof(EventMessage.Timestamp), newTimestampStr } };
//                 await timestampUnit.ReplaceOneAsync(new BsonDocument(), newTimestampDoc, new ReplaceOptions { IsUpsert = true });
//             }

//             await Task.Delay(timeOutSettings.SleepTime);
//         }
//     }
// }