using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Squirrel.Database.Entities
{
    public class RawStreamDataUpdate
    {
        [BsonId] public string Id;
        [BsonElement("firstSeen")] public Instant FirstSeen;
        [BsonElement("lastSeen")] public Instant LastSeen;
        [BsonElement("payload")] public BsonValue Payload;

        public RawStreamDataUpdate(Instant timestamp, JsonElement payload)
        {
            Id = JsonHash.HashHex(payload);
            Payload = BsonSerializer.Deserialize<BsonValue>(payload.GetRawText());

            FirstSeen = timestamp;
            LastSeen = timestamp;
        }
    }
}
