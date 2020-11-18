// https://github.com/xSke/Chronicler/blob/main/SIBR.Storage.Ingest/Workers/StreamDataWorker.cs
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Squirrel.Ingest.Workers
{
    public class StreamDataWorker : BaseWorker
    {
        private readonly EventStream _eventStream;
        public StreamDataWorker(IServiceProvider services) : base(services)
        {
            HttpClient _client = new HttpClient();
            _eventStream = new EventStream(_client, _logger);
        }

        protected override async Task Run()
        {
            await _eventStream.ReadStream("https://www.blaseball.com/events/streamData", async (data) =>
            {
                try
                {
                    Console.WriteLine(data);
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Error while processing stream data");
                }
            });
        }
    }
}
