using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Ptncafe.ConsoleApp.Test.RabbitMq
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string input = ReadLine.Read("Want publish message? yes/no? ");
            if (input.Equals("yes"))
            {

            }
            await new HostBuilder()
               .ConfigureServices((hostContext, services) =>
               {
                   services.AddLogging(loggingBuilder => {
                       loggingBuilder.AddConsole();
                       loggingBuilder.AddDebug();
                   });
                   services.AddHostedService<ConsumerHostedService>();
               })
               .RunConsoleAsync();
           
             


           
        }

    }
}
