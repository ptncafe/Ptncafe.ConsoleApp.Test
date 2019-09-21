using EasyNetQ;
using Ptncafe.ConsoleApp.Test.RabbitMq.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ptncafe.ConsoleApp.RabbitMq
{
    /// <summary>
    /// https://reindeer.rmq.cloudamqp.com/#/
    /// </summary>
    internal class Program
    {
        private static string _hostConnectString = "amqp://unnshbpd:U3dN66ZQ8GY0muSa3HWp9XRR1keAVI4x@reindeer.rmq.cloudamqp.com/unnshbpd";

        private static async Task Main(string[] args)

        {
            string input = ReadLine.Read("Publish yes/no ");
            Console.WriteLine();

            if (input.Equals("no"))
            {


                Console.ReadLine();
            }

            input = ReadLine.Read("Publish max size? ");
            using (var rabbitMqClient = new RabbitMqClient(_hostConnectString))
            {
                await rabbitMqClient.TestPublish(Convert.ToInt32(input));
            }

            Console.ReadLine();
        }
    }

    public class RabbitMqClient : System.IDisposable
    {
        private string _hostConnectString;//= ;
        private IBus _bus;

        public RabbitMqClient(string hostConnectString)
        {
            _hostConnectString = hostConnectString;
            _bus = RabbitHutch.CreateBus(_hostConnectString);
        }

        public void Dispose()
        {
            _bus.Dispose();
        }

        public async Task TestPublish(int maxSize)
        {
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < maxSize; i++)
            {
                var message = new TextMessage
                {
                    CreatedDate = DateTime.Now,
                    Index = i,
                    Message = $"TestPublish_{i}_{DateTime.Now}"
                };
                Console.WriteLine(ObjectDumper.Dump(message));

                tasks.Add(_bus.PublishAsync(message));
            }
            await Task.WhenAll(tasks);
        }
    }
}