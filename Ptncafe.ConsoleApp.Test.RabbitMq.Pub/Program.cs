using Ptncafe.ConsoleApp.Test.RabbitMq.Model;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ptncafe.ConsoleApp.Test.RabbitMq
{
    internal class Program
    {
        private static readonly string _rabbitMqConnectionString = Constant.RabbitMqConnectionString;
        private static readonly string _topicExchangeName = Constant.TopicExchangeName;//"demo.test.topic.exchange"

        private static async Task Main(string[] args)
        {
            string input = ReadLine.Read("Pls Input how many message u want.");
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Ur input is invalid");
                Console.Read();
            }
            int messagesLength;
            if (int.TryParse(input, out messagesLength) == false)
            {
                Console.WriteLine("Ur input is invalid");
                Console.Read();
            }

            var factory = new ConnectionFactory() { Uri = new Uri(_rabbitMqConnectionString) };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                for (int i = 0; i < messagesLength; i++)
                {
                    MesssageDto messsage = new MesssageDto
                    {
                        CreatedDate = DateTime.Now,
                        Message = $"Message test {input} {DateTime.Now}"
                    };
                    var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(messsage));

                    channel.BasicPublish(exchange: _topicExchangeName,
                                         routingKey: Constant.TopicRoutingKey,
                                         basicProperties: null,
                                         body: body);
                    Console.WriteLine(" [x] Sent {0}", JsonSerializer.Serialize(messsage));
                }
              
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}