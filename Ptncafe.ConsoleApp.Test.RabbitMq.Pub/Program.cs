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
            string input = ReadLine.Read("Pls Input how many message u want.");
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Ur input is invalid");
                Console.Read();
            }
            int messagesLenght;
            if (int.TryParse(input,out messagesLenght) == false)
            {
                Console.WriteLine("Ur input is invalid");
                Console.Read();
            }
                        
        }
    }
}