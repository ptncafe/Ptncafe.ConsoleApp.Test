using GreenPipes;
using MassTransit;
using Ptncafe.ConsoleApp.Test.MassTransit.Model;
using System;
using System.Threading.Tasks;

namespace Ptncafe.ConsoleApp.Test.MassTransit
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(new Uri("rabbitmq://192.168.1.12?prefetch=1"), h =>
                {
                    h.Username("services");
                    h.Password("services");
                });

                sbc.ReceiveEndpoint(host, "Test_Queue", ep =>
                {
                    ep.UseConcurrencyLimit(1);
                    ep.PrefetchCount = 1;
                    ep.Handler<TestMassTransitMessage>(async context =>
                    {
                        await Task.Delay(1);
                        Console.Out.WriteLine($"Received: {context.Message} {DateTime.Now}");
                        return;
                    });
                    ep.SetQueueArgument("prefetch", 1);
                    ep.SetQueueArgument("prefetch_count", 1);
                });
            });

            bus.Start(); // This is important!
            for (int i = 0; i < 10000; i++)
            {
                //bus.Publish(new TestMassTransitMessage { Message = $"Hi test {DateTime.Now}" });
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            bus.Stop();
        }
    }
}