using System.Threading.Tasks;
using DwitTech.WalletService.Core.Services;
using DwitTech.WalletService.Core.Interfaces;
using DwitTech.WalletService.Core.Models;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using DwitTech.WalletService.Core.Events;
using System.Reflection;
using Moq.Protected;
using System.Net;
using System;

namespace DwitTech.WalletService.Core.Tests
{
    public class EmailServiceTests
    {
        [Fact]
        public async Task SendMailAsync_UseHttpFalse_ShouldPublishEmailEvent()
        {
            // Arrange
            var email = new Email
            {
                ToEmail = "recipient@example.com",
                Subject = "Test Email",
                Body = "This is a test email."
            };

            // Mock IConfiguration and IHttpClientFactory
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();

            // Mock IEmailEventPublisher
            var mockEmailEventPublisher = new Mock<IEmailEventPublisher>();
            mockEmailEventPublisher
                .Setup(x => x.PublishEmailEventAsync("SendEmail", email))
                .ReturnsAsync(true); // Simulate successful email event publishing

            // Create an instance of the EmailService with the mocked dependencies
            var emailService = new EmailService(
                mockConfiguration.Object,
                mockHttpClientFactory.Object,
                mockEmailEventPublisher.Object
            );

            // Act
            var result = await emailService.SendMailAsync(email, useHttp: false);

            // Assert
            Assert.True(result); // The email event should be published successfully
            mockEmailEventPublisher.Verify(x => x.PublishEmailEventAsync("SendEmail", email), Times.Once);
        }

        [Fact]
        public async Task SendMailAsync_UseHttpTrue_SendHttpEmailSuccess_ShouldNotPublishEmailEvent()
        {
            // Arrange
            var email = new Email
            {
                ToEmail = "recipient@example.com",
                Subject = "Test Email",
                Body = "This is a test email."
            };

            // Mock IConfiguration and IHttpClientFactory
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();

            // Mock IEmailEventPublisher
            var mockEmailEventPublisher = new Mock<IEmailEventPublisher>();
            mockEmailEventPublisher
                .Setup(x => x.PublishEmailEventAsync("SendEmail", email))
                .ReturnsAsync(true); // Simulate successful email event publishing

            // Mock SendHttpEmailAsync to return true (success)
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            var mockHttpClient = new HttpClient(mockMessageHandler.Object);
            mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(mockHttpClient);

            var emailService = new EmailService(mockConfiguration.Object, mockHttpClientFactory.Object, mockEmailEventPublisher.Object);

            // Act
            var result = await emailService.SendMailAsync(email, useHttp: true);

            // Assert
            Assert.True(result); // The email should be sent successfully via HTTP
            mockEmailEventPublisher.Verify(x => x.PublishEmailEventAsync("SendEmail", email), Times.Never);
        }

        [Fact]
        public async Task SendMailAsync_UseHttpTrue_SendHttpEmailFailure_ShouldPublishEmailEvent()
        {
            // Arrange
            var email = new Email
            {
                ToEmail = "recipient@example.com",
                Subject = "Test Email",
                Body = "This is a test email."
            };

            // Mock IConfiguration and IHttpClientFactory
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();

            // Mock IEmailEventPublisher
            var mockEmailEventPublisher = new Mock<IEmailEventPublisher>();
            mockEmailEventPublisher
                .Setup(x => x.PublishEmailEventAsync("SendEmail", email))
                .ReturnsAsync(true); // Simulate successful email event publishing

            // Mock SendHttpEmailAsync to return false (failure)
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.RequestTimeout));

            var httpClient = new HttpClient(mockMessageHandler.Object);
            mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var emailService = new EmailService(mockConfiguration.Object, mockHttpClientFactory.Object, mockEmailEventPublisher.Object);

            // Act
            var result = await emailService.SendMailAsync(email, useHttp: true);

            // Assert
            Assert.True(result); // The email event should be published successfully as a backup
        }
    }
}