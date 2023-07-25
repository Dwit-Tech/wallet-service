using Confluent.Kafka;
using DwitTech.WalletService.Core.Events;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.WalletService.Core.Tests.Events
{
    public class EmailEventPublisherTest
    {
        [Fact]
        public async Task PublishEmailEventAsync_ReturnsTrueWhenSuccessful()
        {
            // Arrange
            var expectedMessage = new Message<string, string>
            {
                Key = "testKey",
                Value = "test event-data"
            };

            var mockTopic = "email-topic";
            var loggerMock = new Mock<ILogger<EmailEventPublisher>>();
            var producerMock = new Mock<IProducer<string, string>>();
            var dummyResult = new DeliveryResult<string, string>
            {
                Offset = new Offset(123),
                Status = PersistenceStatus.Persisted
            };

            producerMock
                .Setup(p => p.ProduceAsync(It.IsAny<string>(), It.IsAny<Message<string, string>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(dummyResult));

            var eventPublisher = new EmailEventPublisher(producerMock.Object, loggerMock.Object);

            // Act
            var result = await eventPublisher.PublishEmailEventAsync("email-topic", expectedMessage);

            // Assert
            Assert.True(result);
            producerMock.Verify(p => p.ProduceAsync(mockTopic, It.IsAny<Message<string, string>>(), It.IsAny<CancellationToken>()), Times.Once);
            producerMock.Verify(p => p.Flush(It.IsAny<TimeSpan>()), Times.Once);
        }

        [Fact]
        public async Task PublishEmailEventAsync_ThrowsProduceExceptionWhenSendingFails()
        {
            // Arrange
            var mockTopic = "email-topic";
            var loggerMock = new Mock<ILogger<EmailEventPublisher>>();
            var producerMock = new Mock<IProducer<string, string>>();
            producerMock.Setup(p => p.ProduceAsync(It.IsAny<string>(), It.IsAny<Message<string, string>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ProduceException<string, string>(new Error(ErrorCode.Unknown, "Error"), new DeliveryResult<string, string>()));

            var eventPublisher = new EmailEventPublisher(producerMock.Object, loggerMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ProduceException<string, string>>(() => eventPublisher.PublishEmailEventAsync("email-topic", "event-data"));
            producerMock.Verify(p => p.ProduceAsync(mockTopic, It.IsAny<Message<string, string>>(), It.IsAny<CancellationToken>()),
                Times.Once);
            producerMock.Verify(p => p.Flush(It.IsAny<TimeSpan>()), Times.Never);
        }
    }
}
