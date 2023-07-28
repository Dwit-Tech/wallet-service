
using DwitTech.WalletService.Data.Repositories;
using DwitTech.WalletService.Core.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.AspNetCore.Http;
using DwitTech.WalletService.Data.Entities;
using System.Security.Claims;
using DwitTech.WalletService.Core.Exceptions;

namespace DwitTech.WalletService.Core.Tests.Services
{
    public class UserWalletServiceTests
    {
        [Fact]
        public async Task CheckCreateWalletReturnsTrueWhenSuccessful()
        {
            //Arrange
            var userId = 1;
            var validCurrencyCode = "USD";
            Currency currency = new() { Id = 1, Code = "NGN", Name = "Nigerian Naira", CreatedBy = "Magician" };

            var mockWalletRepo = new Mock<IWalletRepository>();
            var mockLogger = new Mock<ILogger<UserWalletService>>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            mockWalletRepo.Setup(r => r.CreateWallet(It.IsAny<Wallet>())).Returns(Task.CompletedTask);
            mockWalletRepo.Setup(r => r.CheckCurrencyCode(It.IsAny<string>())).Returns(true);
            mockWalletRepo.Setup(r => r.CheckWalletExists(It.IsAny<int>(), It.IsAny<string>())).Returns(false);
            mockWalletRepo.Setup(r => r.GetCurrencyIdByCode(It.IsAny<string>())).ReturnsAsync(currency);

            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim("UserId", userId.ToString())
                }))
            };
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(httpContext);

            //Act
            var userWalletService = new UserWalletService(mockWalletRepo.Object, mockLogger.Object, mockHttpContextAccessor.Object);
            var result = await userWalletService.CreateWallet(userId, validCurrencyCode);

            //Assert
            Assert.True(result);
            mockWalletRepo.Verify(r => r.CreateWallet(It.IsAny<Wallet>()), Times.Once);
            mockWalletRepo.Verify(r => r.CheckCurrencyCode(validCurrencyCode), Times.Once);
            mockWalletRepo.Verify(r => r.CheckWalletExists(userId, It.IsAny<string>()), Times.Once);
            mockWalletRepo.Verify(r => r.GetCurrencyIdByCode(validCurrencyCode), Times.Once);
        }

        [Fact]
        public async Task CheckValidateCreateWalletRequest_ThrowsArgumentOutOfRangeException_WhenCurrencyCodeIsInvalid()
        {
            //Arrange
            var userId = 1;
            var invalidCurrencyCode = "RRR";
            var mockWalletRepo = new Mock<IWalletRepository>();
            var mockLogger = new Mock<ILogger<UserWalletService>>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            mockWalletRepo.Setup(r => r.CreateWallet(It.IsAny<Wallet>())).Returns(Task.CompletedTask);
            mockWalletRepo.Setup(r => r.CheckCurrencyCode(It.IsAny<string>())).Returns(false);
            mockWalletRepo.Setup(r => r.CheckWalletExists(It.IsAny<int>(), It.IsAny<string>())).Returns(false);

            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim("UserId", userId.ToString())
                }))
            };
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(httpContext);

            //Act
            var userWalletService = new UserWalletService(mockWalletRepo.Object, mockLogger.Object, mockHttpContextAccessor.Object);

            //Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => userWalletService.CreateWallet(userId, invalidCurrencyCode));
            mockWalletRepo.Verify(r => r.GetCurrencyIdByCode(invalidCurrencyCode), Times.Never);
        }

        [Fact]
        public async Task CheckValidateCreateWalletRequest_ThrowsDuplicateRequestException_WhenWalletExists()
        {
            //Arrange
            var userId = 1;
            var validCurrencyCode = "USD";
            var mockWalletRepo = new Mock<IWalletRepository>();
            var mockLogger = new Mock<ILogger<UserWalletService>>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            mockWalletRepo.Setup(r => r.CreateWallet(It.IsAny<Wallet>())).Returns(Task.CompletedTask);
            mockWalletRepo.Setup(r => r.CheckCurrencyCode(It.IsAny<string>())).Returns(true);
            mockWalletRepo.Setup(r => r.CheckWalletExists(It.IsAny<int>(), It.IsAny<string>())).Returns(true);

            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim("UserId", userId.ToString())
                }))
            };
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(httpContext);

            //Act
            var userWalletService = new UserWalletService(mockWalletRepo.Object, mockLogger.Object, mockHttpContextAccessor.Object);

            //Assert
            await Assert.ThrowsAsync<DuplicateRequestException>(() => userWalletService.CreateWallet(userId, validCurrencyCode));
            mockWalletRepo.Verify(r => r.GetCurrencyIdByCode(validCurrencyCode), Times.Never);
        }

        [InlineData(0, "USD")]
        [InlineData(1, " ")]
        [Theory]
        public async Task CheckCreateWallet_ThrowsArgumentNullException_WhenParametersHaveNoValue(int userId, 
            string currencyCode)
        {
            //Arrange
            var mockWalletRepo = new Mock<IWalletRepository>();
            var mockLogger = new Mock<ILogger<UserWalletService>>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            mockWalletRepo.Setup(r => r.CreateWallet(It.IsAny<Wallet>())).Returns(Task.CompletedTask);
            mockWalletRepo.Setup(r => r.CheckCurrencyCode(It.IsAny<string>())).Returns(true);
            mockWalletRepo.Setup(r => r.CheckWalletExists(It.IsAny<int>(), It.IsAny<string>())).Returns(true);

            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim("UserId", userId.ToString())
                }))
            };
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(httpContext);

            //Act
            var userWalletService = new UserWalletService(mockWalletRepo.Object, mockLogger.Object, mockHttpContextAccessor.Object);

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => userWalletService.CreateWallet(userId, currencyCode));
        }

        [Fact]
        public async Task CheckCreateWallet_ThrowsNullReferenceException_WhenUserIdIsMissingFromToken()
        {
            //Arrange
            var userId = 1;
            var validCurrencyCode = "USD";
            var mockWalletRepo = new Mock<IWalletRepository>();
            var mockLogger = new Mock<ILogger<UserWalletService>>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            mockWalletRepo.Setup(r => r.CreateWallet(It.IsAny<Wallet>())).Returns(Task.CompletedTask);
            mockWalletRepo.Setup(r => r.CheckCurrencyCode(It.IsAny<string>())).Returns(true);
            mockWalletRepo.Setup(r => r.CheckWalletExists(It.IsAny<int>(), It.IsAny<string>())).Returns(true);

            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(new DefaultHttpContext());

            //Act
            var userWalletService = new UserWalletService(mockWalletRepo.Object, mockLogger.Object, mockHttpContextAccessor.Object);

            //Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => userWalletService.CreateWallet(userId, validCurrencyCode));
        }

        [Fact]
        public async Task CheckValidateCreateWalletRequest_ThrowsOperationCancelledException_WhenGetCurrencyIdByCodeThrowsSameException()
        {
            //Arrange
            var userId = 1;
            var validCurrencyCode = "USD";
            var mockWalletRepo = new Mock<IWalletRepository>();
            var mockLogger = new Mock<ILogger<UserWalletService>>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            mockWalletRepo.Setup(r => r.CreateWallet(It.IsAny<Wallet>())).Returns(Task.CompletedTask);
            mockWalletRepo.Setup(r => r.CheckCurrencyCode(It.IsAny<string>())).Returns(true);
            mockWalletRepo.Setup(r => r.CheckWalletExists(It.IsAny<int>(), It.IsAny<string>())).Returns(false);
            mockWalletRepo.Setup(r => r.GetCurrencyIdByCode(It.IsAny<string>())).Throws<OperationCanceledException>();

            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim("UserId", userId.ToString())
                }))
            };
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(httpContext);

            //Act
            var userWalletService = new UserWalletService(mockWalletRepo.Object, mockLogger.Object, mockHttpContextAccessor.Object);

            //Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() => userWalletService.CreateWallet(userId, validCurrencyCode));
            mockWalletRepo.Verify(r => r.GetCurrencyIdByCode(validCurrencyCode), Times.Once);
        }
    }    
}
