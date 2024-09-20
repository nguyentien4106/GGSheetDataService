using Confluent.Kafka;
using DataService.Core.Settings;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService.Core.Messaging
{
    public class RabbitMQProducer
    {
        RabbitMQParams _rabbit;

        public RabbitMQProducer(RabbitMQParams rabbit)
        {
            _rabbit = rabbit;
        }

        public void SendMessage(string queue, string message)
        {
            var factory = new ConnectionFactory() { HostName = _rabbit.HostName };
            using var connection = factory.CreateConnection();

            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: queue,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var body = Encoding.UTF8.GetBytes(message);
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: "",
                                 routingKey: queue,
                                 basicProperties: properties,
                                 body: body);
        }
    }
}
