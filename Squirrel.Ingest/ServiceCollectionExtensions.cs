using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Squirrel.Ingest.Workers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Squirrel.Ingest
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSerilog(this IServiceCollection services)
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Async(async =>
                {
                    async.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}", restrictedToMinimumLevel: LogEventLevel.Debug);
                })
                .CreateLogger();
            Log.Logger = logger;

            services.AddSingleton<ILogger>(logger);
            return services;
        }

        public static IServiceCollection AddSquirrelIngest(this IServiceCollection services)
        {
            return services
                .AddSingleton(_ => {
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent",
                        "Squirrel/0.1 (https://github.com/galenguyer/squirrel, MasterChief_John-117#1911 on Discord)");
                    return client;
                })
                .AddSingleton<EventStream>()
                .AddSingleton<StreamDataWorker>();
        }
    }
}
