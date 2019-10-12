using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ptncafe.ConsoleApp.Test.RabbitMq.Model;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Ptncafe.ConsoleApp.Test.RabbitMq
{
    public class ConsumerHostedService : IHostedService, System.IDisposable
    {
        private readonly ILogger<ConsumerHostedService> _logger;

        #region config

        private readonly string _rabbitMqConnectionString = Constant.RabbitMqConnectionString;

        #endregion config

        /// <summary>
        /// RabbitMQ connection
        /// </summary>
        private IConnection _connection;

        private IModel _channelFactory;
        private List<IModel> _channels;

        public ConsumerHostedService(ILogger<ConsumerHostedService> logger)
        {
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(ConsumerHostedService)} StartAsync");

            await InitRabbitMQ();

            await TopicQueue(cancellationToken);
            await FanoutQueue(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var item in _channels)
            {
                item.Close();
            }
            _channelFactory.Close();
            _connection.Close();
            _logger.LogWarning($"{nameof(ConsumerHostedService)} StopAsync");
            return Task.CompletedTask;
        }

        /// <summary>
        /// https://www.cloudamqp.com/blog/2015-09-03-part4-rabbitmq-for-beginners-exchanges-routing-keys-bindings.html
        /// </summary>
        /// <returns></returns>
        private Task InitRabbitMQ()
        {
            var connectionFactory = new ConnectionFactory()
            {
                Uri = new Uri(_rabbitMqConnectionString)
            };

            // create connection
            _connection = connectionFactory.CreateConnection();

            // create channel
            _channelFactory = _connection.CreateModel();

            _channelFactory.ExchangeDeclare(Constant.TopicExchangeName, ExchangeType.Topic);//"demo.test.topic.exchange"
            _channelFactory.ExchangeDeclare(Constant.FanoutExchangeName, ExchangeType.Fanout);//"demo.test.topic.fanout"
            _channelFactory.ExchangeDeclare(Constant.DirectExchangeName, ExchangeType.Direct);//"demo.test.topic.direct"

            _logger.LogInformation($"InitRabbitMQ ExchangeDeclare done");

            _channelFactory.BasicQos(0, 10, true);
            _channels = new List<IModel>();
            return Task.CompletedTask;
        }

        private async Task TopicQueue(CancellationToken cancellationToken)
        {
            await ConsumerAsync<MesssageDto>(queueName: "demo.test.topic.queue.noti.web"
               , exchangeName: Constant.TopicExchangeName
               , routingKey: Constant.Topic_Noti_Queue_RoutingKey//"demo.test.topic.*.noti"
               , prefetchCount: 1
               , (message) =>
               {
                   Thread.Sleep(2000);
                   //throw new Exception($"demo.test.topic.queue.noti_web Error {DateTime.Now}  Exception  {message.Message}  {message.CreatedDate}");
                   Console.WriteLine($"web {Constant.Topic_Noti_Queue_RoutingKey} {DateTime.Now} => {message.Message}  {message.CreatedDate}");
               }, cancellationToken);

            await ConsumerAsync<MesssageDto>(queueName: "demo.test.topic.queue.noti.mobile"
               , exchangeName: Constant.TopicExchangeName
               , routingKey: Constant.Topic_Noti_Queue_RoutingKey//"demo.test.topic.*.noti"
               , prefetchCount: 1
               , (message) =>
               {
                   Thread.Sleep(1000);
                   //throw new Exception($"demo.test.topic.queue.noti_web Error {DateTime.Now}  Exception  {message.Message}  {message.CreatedDate}");
                   Console.WriteLine($"mobile {Constant.Topic_Noti_Queue_RoutingKey} {DateTime.Now} => {message.Message}  {message.CreatedDate}");
               }, cancellationToken);
        }

        private async Task FanoutQueue(CancellationToken cancellationToken)
        {
            await ConsumerAsync<MesssageDto>(queueName: "demo.test.fanout.queue.noti.web"
               , exchangeName: Constant.FanoutExchangeName
               , routingKey: string.Empty
               , prefetchCount: 1
               , (message) =>
               {
                   Thread.Sleep(2000);
                   //throw new Exception($"demo.test.topic.queue.noti_web Error {DateTime.Now}  Exception  {message.Message}  {message.CreatedDate}");
                   Console.WriteLine($"web {Constant.FanoutExchangeName} {DateTime.Now} => {message.Message}  {message.CreatedDate}");
               }, cancellationToken);

            await ConsumerAsync<MesssageDto>(queueName: "demo.test.fanout.queue.noti.mobile"
               , exchangeName: Constant.FanoutExchangeName
               , routingKey: string.Empty
               , prefetchCount: 1
               , (message) =>
               {
                   Thread.Sleep(1000);
                   //throw new Exception($"demo.test.topic.queue.noti_web Error {DateTime.Now}  Exception  {message.Message}  {message.CreatedDate}");
                   Console.WriteLine($"mobile {Constant.FanoutExchangeName} {DateTime.Now} => {message.Message}  {message.CreatedDate}");
               }, cancellationToken);
        }
        private async Task DirectQueue(CancellationToken cancellationToken)
        {
            await ConsumerAsync<MesssageDto>(queueName: "demo.test.fanout.queue.noti.web"
               , exchangeName: Constant.DirectExchangeName
               , routingKey: string.Empty
               , prefetchCount: 1
               , (message) =>
               {
                   Thread.Sleep(2000);
                   //throw new Exception($"demo.test.topic.queue.noti_web Error {DateTime.Now}  Exception  {message.Message}  {message.CreatedDate}");
                   Console.WriteLine($"web {Constant.FanoutExchangeName} {DateTime.Now} => {message.Message}  {message.CreatedDate}");
               }, cancellationToken);

            await ConsumerAsync<MesssageDto>(queueName: "demo.test.fanout.queue.noti.mobile"
               , exchangeName: Constant.DirectExchangeName
               , routingKey: string.Empty
               , prefetchCount: 1
               , (message) =>
               {
                   Thread.Sleep(1000);
                   //throw new Exception($"demo.test.topic.queue.noti_web Error {DateTime.Now}  Exception  {message.Message}  {message.CreatedDate}");
                   Console.WriteLine($"mobile {Constant.FanoutExchangeName} {DateTime.Now} => {message.Message}  {message.CreatedDate}");
               }, cancellationToken);
        }

        private async Task ConsumerAsync<T>(string queueName
            , string exchangeName
            , string routingKey
            , ushort prefetchCount
            , Action<T> action
            , CancellationToken cancellationToken)
        {
            var channel = _connection.CreateModel();

            channel.QueueDeclare(queueName, false, false, false, null);
            channel.QueueBind(queueName, exchangeName, routingKey, null);
            cancellationToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, basicDeliverEventArgs) =>
           {
               try
               {
                   var body = basicDeliverEventArgs.Body;
                   var message = Encoding.UTF8.GetString(body);
                   var data = JsonSerializer.Deserialize<T>(body);
                   action(data);
                   channel.BasicAck(basicDeliverEventArgs.DeliveryTag, false);
               }
               catch (Exception ex)
               {
                   _logger.LogError($"Process Error {DateTime.Now} => {basicDeliverEventArgs.Body} {DateTime.Now} {ex}");
                   channel.BasicReject(basicDeliverEventArgs.DeliveryTag, true);
               }
           };
            channel.BasicQos(0, prefetchCount, false);
            channel.BasicConsume(queue: queueName, consumer: consumer);
            _channels.Add(channel);
        }

        public void Dispose()
        {
            foreach (var item in _channels)
            {
                item.Dispose();
            }
            _channelFactory.Dispose();

            _connection.Dispose();
        }
    }
}