using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMqDi.Configuration;

namespace RabbitMqDi.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly RabbitMqService _rabbitMq;

        public IndexModel(ILogger<IndexModel> logger, RabbitMqService rabbitMq)
        {
            _logger = logger;
            _rabbitMq = rabbitMq;
        }

        public void OnGet([FromServices] RabbitMqService rabbitMq)
        {
            rabbitMq.Channel.QueueDeclare(queue: "TestQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            var message = Encoding.UTF8.GetBytes("This is Working!!!!");
            rabbitMq.Channel.BasicPublish("", routingKey: "TestQueue", basicProperties: null, body: message);
        }


        public void OnGetLogMessages()
        {
            var result = "";
            _rabbitMq.Channel.QueueDeclare(queue: "TestQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            var consumer = _rabbitMq.CreateConsumer();
            consumer.Received += ConsumerOnReceived;
            _rabbitMq.Channel.BasicConsume(queue: "TestQueue", autoAck: true, consumer: consumer);
        }

        private void ConsumerOnReceived(object sender, BasicDeliverEventArgs e)
        {
            var body = e.Body;
            var message = Encoding.UTF8.GetString(body.ToArray());
            _logger.LogWarning("New Message "+message);
        }
    }
}
