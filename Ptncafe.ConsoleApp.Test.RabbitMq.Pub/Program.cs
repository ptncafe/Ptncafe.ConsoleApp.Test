using Ptncafe.ConsoleApp.Test.RabbitMq.Model;
using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ptncafe.ConsoleApp.Test.RabbitMq
{
    internal class Program
    {
        private static readonly string _rabbitMqConnectionString = Constant.RabbitMqConnectionString;
        private static readonly Random random = new Random();

        private static async Task Main(string[] args)
        {
            Console.WriteLine("Pls Input exchange type. topic? fanout? direct?");
            string exchangType = Console.ReadLine();

            Console.WriteLine("Pls Input how many message u want.");
            string input = Console.ReadLine();
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

            switch (exchangType)
            {
                case "topic":
                    Topic(messagesLength);
                    break;

                default:
                    break;
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private static void Topic(int messagesLength)
        {
            var factory = new ConnectionFactory() { Uri = new Uri(_rabbitMqConnectionString) };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                for (int i = 0; i < messagesLength; i++)
                {
                    var randomString = RandomString(8);
                    channel.BasicPublish(exchange: Constant.TopicExchangeName,
                                         routingKey: Constant.Topic_Noti_Order_Publish_RoutingKey,
                                         basicProperties: null,
                                         body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new MesssageDto
                                         {
                                             CreatedDate = DateTime.Now,
                                             Message = $"Message test {Constant.Topic_Noti_Order_Publish_RoutingKey} {randomString} {DateTime.Now}"
                                         })));

                    channel.BasicPublish(exchange: Constant.TopicExchangeName,
                                           routingKey: Constant.Topic_Noti_Product_Publish_RoutingKey,
                                           basicProperties: null,
                                           body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new MesssageDto
                                           {
                                               CreatedDate = DateTime.Now,
                                               Message = $"Message test {Constant.Topic_Noti_Product_Publish_RoutingKey} {randomString} {DateTime.Now}"
                                           })));


                    Console.WriteLine($"BasicPublish {DateTime.Now} => {Constant.TopicExchangeName}  {0}", i);
                }
            }
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}