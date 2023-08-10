using DwitTech.WalletService.Core.Events;
using DwitTech.WalletService.Core.Interfaces;
using DwitTech.WalletService.Core.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.WalletService.Core.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IEmailEventPublisher _eventPublisher;

        public EmailService(IConfiguration configuration, IHttpClientFactory httpClientFactory, IEmailEventPublisher eventPublisher)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _eventPublisher = eventPublisher;
        }

        private async Task<bool> SendHttpEmailAsync(Email email)
        {
            return await Task.FromResult(true);
        }

        public async Task<bool> SendMailAsync(Email email, bool useHttp = false)
        {
            const string topicName = "SendEmail";

            if (useHttp)
            {
                var status = await SendHttpEmailAsync(email);

                if (!status)
                {
                    return await _eventPublisher.PublishEmailEventAsync(topicName, email);
                }
                return status;
            }
            return await _eventPublisher.PublishEmailEventAsync(topicName, email);
        }
    }
}
