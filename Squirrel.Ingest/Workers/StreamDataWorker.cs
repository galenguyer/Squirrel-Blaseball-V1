// https://github.com/xSke/Chronicler/blob/main/SIBR.Storage.Ingest/Workers/StreamDataWorker.cs
using Microsoft.Extensions.DependencyInjection;
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
            _eventStream = services.GetRequiredService<EventStream>();
        }

        protected override async Task Run()
        {
            await _eventStream.ReadStream("https://www.blaseball.com/events/streamData", async (data) =>
            {
                try
                {

                }
                catch (Exception e)
                {
                    _logger.Error(e, "Error while processing stream data");
                }
            });
        }
    }
}
