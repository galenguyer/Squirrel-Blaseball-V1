using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MongoDb.Bson.NodaTime;
using MongoDB.Driver;
using NodaTime;
using Serilog;
using Squirrel.Database.Entities;

namespace Squirrel.Database
{
    public class MongoDBClient
    {
        MongoClient _client;
        ILogger _logger;

        IMongoCollection<RawStreamDataUpdate> _rawUpdates;
        IMongoCollection<GameUpdate> _gameUpdates;
        public MongoDBClient(IServiceProvider services)
        {
            string connectionString = Environment.GetEnvironmentVariable("MONGO_URI");
            if (string.IsNullOrEmpty(connectionString))
                throw new Exception("Need mongodb URI in MONGO_URI env");

            _client = new MongoClient(connectionString);
            _logger = services.GetRequiredService<ILogger>();
            NodaTimeSerializers.Register();

            var db = _client.GetDatabase("streamData");
            _rawUpdates = db.GetCollection<RawStreamDataUpdate>("raw");
            _gameUpdates = db.GetCollection<GameUpdate>("games");
        }

        public async Task WriteRaw(Instant instant, JsonDocument doc)
        {
            var update = new RawStreamDataUpdate(instant, doc.RootElement);

            var filter = Builders<RawStreamDataUpdate>.Filter.Eq(x => x.Id, update.Id);

            var model = Builders<RawStreamDataUpdate>.Update
                .SetOnInsert(x => x.Payload, update.Payload)
                .Min(x => x.FirstSeen, update.FirstSeen)
                .Max(x => x.LastSeen, update.LastSeen);
            await _rawUpdates.UpdateOneAsync(filter, model, new UpdateOptions { IsUpsert = true });

            _logger.Information("Upserted streamData.raw.{Hash}", update.Id);
        }

        public async Task WriteGame(Instant instant, JsonElement elem)
        {
            var update = new GameUpdate(instant, elem);

            var filter = Builders<GameUpdate>.Filter.Eq(x => x.Id, update.Id);

            var model = Builders<GameUpdate>.Update
                .SetOnInsert(x => x.Payload, update.Payload)
                .Min(x => x.FirstSeen, update.FirstSeen)
                .Max(x => x.LastSeen, update.LastSeen);
            await _gameUpdates.UpdateOneAsync(filter, model, new UpdateOptions { IsUpsert = true });

            _logger.Information("Upserted streamData.games.{Hash}", update.Id);
        }
    }
}
