﻿namespace RabbitMQAzureMetrics
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using RabbitMQAzureMetrics.Configuration;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var hostBuilder = Setup(args);
            using (var host = hostBuilder.Build())
            {
                await host.StartAsync();
                await host.WaitForShutdownAsync();
            }
        }

        private static IHostBuilder Setup(string[] args)
        {
            var hostBuilder = new HostBuilder()
                .ConfigureLogging((ctx, logging) =>
                {
                    logging.AddConfiguration(ctx.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                })
                .ConfigureAppConfiguration((ctx, config) =>
                {
                    config.AddJsonFile("appsettings.json", false);
                    config.AddJsonFile("appsettings.local.json", true);
                    config.AddCommandLine(args);
                    config.AddEnvironmentVariables();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging();
                    var config = new RabbitMetricsConfiguration();
                    hostContext.Configuration.Bind(config);
                    services.AddRabbitMqMetrics(config);
                })
                .UseWindowsService();

            return hostBuilder;
        }
    }
}
