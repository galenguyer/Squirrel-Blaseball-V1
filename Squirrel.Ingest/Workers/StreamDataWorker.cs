// https://github.com/xSke/Chronicler/blob/main/SIBR.Storage.Ingest/Workers/StreamDataWorker.cs
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Serilog;
using Squirrel.Database;
using Squirrel.Database.Entities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Squirrel.Ingest.Workers
{
    public class StreamDataWorker : BaseWorker
    {
        private readonly EventStream _eventStream;
        private readonly MongoDBClient _db;
        private readonly IClock _clock;

        public StreamDataWorker(IServiceProvider services) : base(services)
        {
            _eventStream = services.GetRequiredService<EventStream>();
            _db = services.GetRequiredService<MongoDBClient>();
            _clock = services.GetRequiredService<IClock>();
        }

        protected override async Task Run()
        {
            await _eventStream.ReadStream("https://www.blaseball.com/events/streamData", async (data) =>
            {
                try
                {
                    var doc = JsonDocument.Parse(data);
                    var update = new RawStreamDataUpdate(_clock.GetCurrentInstant(), doc.RootElement);
                    await _db.WriteRaw(update);
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Error while processing stream data");
                }
            });
        }
    }
}
