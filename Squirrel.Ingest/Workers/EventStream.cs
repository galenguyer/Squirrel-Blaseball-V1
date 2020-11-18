// https://github.com/xSke/Chronicler/blob/main/SIBR.Storage.Ingest/EventStream.cs
using Serilog;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Squirrel.Ingest.Workers
{
    public class EventStream
    {
        private readonly string Prefix = "data: ";

        private readonly HttpClient _client;
        private readonly ILogger _logger;

        public EventStream(HttpClient client, ILogger logger)
        {
            _client = client;
            _logger = logger.ForContext<EventStream>();
        }

        public async Task ReadStream(string uri, Action<string> callback)
        {
            while (true)
            {
                try
                {
                    _logger.Information("Connecting to stream URL {Url}", uri);
                    System.Threading.Thread.Sleep(10000);
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Error while reading from stream {Url}", uri);
                }
            }
        }
    }
}
