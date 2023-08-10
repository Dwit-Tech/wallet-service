using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.WalletService.Core.Events
{
    public class EmailEventPublisher : IEmailEventPublisher
    {
        private readonly IProducer<string, string> _producer;
        private readonly ILogger<EmailEventPublisher> _logger;

        public EmailEventPublisher(IProducer<string, string> producer, ILogger<EmailEventPublisher> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        public async Task<bool> PublishEmailEventAsync<T>(string topic, T eventData)
        {
            try
            {
                var serializedData = JsonConvert.SerializeObject(eventData);
                await _producer.ProduceAsync(topic, new Message<string, string>
                {
                    Value = serializedData
                });

                _producer.Flush(TimeSpan.FromSeconds(3));
                _logger.LogInformation("Email published successfully");
                return true;
            }
            catch (ProduceException<string, string> ex)
            {
                _logger.LogError(ex, "Error! Unable to publish event!");
                throw;
            }
        }
    }
}
