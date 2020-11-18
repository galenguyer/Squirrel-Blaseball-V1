// https://github.com/xSke/Chronicler/blob/main/SIBR.Storage.Ingest/EventStream.cs
using Serilog;
using System;
using System.IO;
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
                    var response = await _client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
                    await using var stream = await response.Content.ReadAsStreamAsync();

                    _logger.Information("Connected to stream, receiving data");

                    string str;
                    using var reader = new StreamReader(stream);
                    while ((str = await reader.ReadLineAsync()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(str))
                            continue;

                        if (!str.StartsWith(Prefix))
                            continue;

                        callback(str.Substring(Prefix.Length));
                    }
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Error while reading from stream {Url}", uri);
                }
            }
        }
    }
}
