﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Squirrel.Ingest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IServiceProvider services = new ServiceCollection()
                .AddSerilog()
                .BuildServiceProvider();

            await HandleIngest(services);
        }

        private static async Task HandleIngest(IServiceProvider services)
        {
            var logger = services.GetRequiredService<ILogger>();
            logger.Information("Starting Ingress Workers");
            System.Threading.Thread.Sleep(10000);
        }
    }
}
