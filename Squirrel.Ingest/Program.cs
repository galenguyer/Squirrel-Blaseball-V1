using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Squirrel.Database;
using Squirrel.Ingest.Workers;

namespace Squirrel.Ingest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IServiceProvider services = new ServiceCollection()
                .AddSerilog()
                .AddSquirrelIngest()
                .AddSingleton<MongoDBClient>()
                .BuildServiceProvider();

            await HandleIngest(services);
        }

        private static async Task HandleIngest(IServiceProvider services)
        {
            var logger = services.GetRequiredService<ILogger>();
            logger.Information("Starting Ingress Workers");
            await new StreamDataWorker(services).Start();
        }
    }
}
