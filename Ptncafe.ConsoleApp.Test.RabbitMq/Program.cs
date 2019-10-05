using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Ptncafe.ConsoleApp.Test.RabbitMq
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            await new HostBuilder()
               .ConfigureServices((hostContext, services) =>
               {
                   services.AddLogging(loggingBuilder =>
                   {
                       loggingBuilder.AddConsole();
                       loggingBuilder.AddDebug();
                   });
                   services.AddHostedService<ConsumerHostedService>();
               })
               .RunConsoleAsync();
        }
    }
}