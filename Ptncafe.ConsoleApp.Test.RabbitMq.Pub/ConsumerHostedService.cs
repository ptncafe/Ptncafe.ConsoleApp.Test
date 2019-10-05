﻿using Microsoft.Extensions.Hosting;
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
        private readonly string _topicExchangeName = Constant.TopicExchangeName;//"demo.test.topic.exchange"

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
            await ConsumerAsync<MesssageDto>(queueName: "demo.test.topic.queue"
                , exchangeName: _topicExchangeName
                , routingKey: "demo.test.topic.*"
                , prefetchCount: 1
                , async (message) =>
            {
                await Task.Delay(1000);
                _logger.LogDebug($"demo.test.topic.queue {message} ");
            }, cancellationToken);
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


            _channelFactory.ExchangeDeclare(_topicExchangeName, ExchangeType.Topic);//"demo.test.topic.exchange"
            _logger.LogInformation($"InitRabbitMQ ExchangeDeclare {_topicExchangeName} done");


            _channelFactory.BasicQos(0, 10, true);
            _channels = new List<IModel>();
            return Task.CompletedTask;
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
            consumer.Received += async (model, basicDeliverEventArgs) =>
            {
                var body = basicDeliverEventArgs.Body;
                var message = Encoding.UTF8.GetString(body);
                var data = JsonSerializer.Deserialize<T>(body);
                try
                {
                    action(data);
                    channel.BasicAck(basicDeliverEventArgs.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogDebug($"Process Error => {message} {DateTime.Now} {ex}");
                    channel.BasicNack(basicDeliverEventArgs.DeliveryTag, false, false);
                }
            };
            channel.BasicQos(0, prefetchCount, false);
            channel.BasicConsume(queue: queueName, consumer: consumer);
            _channels.Add(channel);
        }

        private Task<bool> Process(string message)
        {
            _logger.LogDebug($"Process {message} {DateTime.Now}");
            return Task.FromResult(true);
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