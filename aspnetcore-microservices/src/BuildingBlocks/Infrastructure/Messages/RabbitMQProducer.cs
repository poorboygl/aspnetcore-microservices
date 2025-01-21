using Contracts.Common.Interfaces;
using Contracts.Messages;
using Infrastructure.Common;
using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using System.Text;

namespace Infrastructure.Messages
{
    public class RabbitMQProducer : IMessageProducer
    {
        private readonly ISerializerService _serializerService;

        public RabbitMQProducer(
            ISerializerService serializerService
        )
        {
            _serializerService = serializerService;
        }

        public void SendMessage<T>(T message)
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = "localhost"
            };

            var connection = connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare("orders", exclusive: false);

            var jsonData = _serializerService.Serialize(message); // serialize message đó về kiểu json.
            var body = Encoding.UTF8.GetBytes(jsonData); // Chuyển các string đó về ký tự byte

            channel.BasicPublish(exchange: "", routingKey: "orders", body: body);
        }
    }
}