using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MongoDb.Bson.NodaTime;
using MongoDB.Driver;
using Serilog;
using Squirrel.Database.Entities;

namespace Squirrel.Database
{
    public class MongoDBClient
    {
        MongoClient _client;
        ILogger _logger;

        IMongoCollection<RawStreamDataUpdate> _rawUpdates;
        public MongoDBClient(IServiceProvider services)
        {
            _client = new MongoClient(File.ReadAllText(".mongo"));
            _logger = services.GetRequiredService<ILogger>();
            NodaTimeSerializers.Register();

            var db = _client.GetDatabase("streamData");
            _rawUpdates = db.GetCollection<RawStreamDataUpdate>("raw");
        }

        public async Task WriteRaw(RawStreamDataUpdate update)
        {
            var filter = Builders<RawStreamDataUpdate>.Filter.Eq(x => x.Id, update.Id);

            var model = Builders<RawStreamDataUpdate>.Update
                .SetOnInsert(x => x.Payload, update.Payload)
                .Min(x => x.FirstSeen, update.FirstSeen)
                .Max(x => x.LastSeen, update.LastSeen);
            await _rawUpdates.UpdateOneAsync(filter, model, new UpdateOptions { IsUpsert = true });
        }
    }
}
