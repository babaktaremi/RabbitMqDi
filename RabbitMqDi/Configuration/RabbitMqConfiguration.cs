using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMqDi.Configuration
{
    public static class RabbitMqExtensions
    {
        public static void AddRabbitMq(this IServiceCollection services, string url)
        {
            services.Configure<RabbitMqSetting>(r => r.RabbitMqUrl = url);
            services.AddTransient<RabbitMqService>();
        }
    }


    public class RabbitMqSetting
    {
        public string RabbitMqUrl { get; set; }
    }

    public class RabbitMqService
    {
        private RabbitMqSetting _settings;
        public IModel Channel { get; private set; }
        public RabbitMqService(IOptions<RabbitMqSetting> options)
        {
            _settings = options.Value;

            var factory = new ConnectionFactory() { HostName = _settings.RabbitMqUrl };
            var connection = factory.CreateConnection();
             Channel = connection.CreateModel();
            // Consumer= new EventingBasicConsumer(this.Channel);
        }

        public AsyncEventingBasicConsumer CreateConsumerAsync()
        {
            return new AsyncEventingBasicConsumer(Channel);
        }

        public EventingBasicConsumer CreateConsumer()
        {
            return new EventingBasicConsumer(Channel);
        }
    }
}
