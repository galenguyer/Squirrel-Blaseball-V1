// https://github.com/xSke/Chronicler/blob/main/SIBR.Storage.Ingest/BaseWorker.cs
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Squirrel.Ingest
{
    public abstract class BaseWorker
    {
        protected readonly ILogger _logger;

        protected BaseWorker(IServiceProvider services)
        {
            _logger = services.GetRequiredService<ILogger>()
                .ForContext(GetType());
        }

        protected abstract Task Run();

        public async Task Start()
        {
            while (true)
            {
                try
                {
                    _logger.Information("Starting ingest worker {WorkerType}", GetType().Name);
                    await Run();
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Error while running worker {WorkerType}", GetType().Name);
                }
            }
        }
    }
}
