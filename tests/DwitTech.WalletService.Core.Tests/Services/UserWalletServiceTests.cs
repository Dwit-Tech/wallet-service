
using DwitTech.WalletService.Data.Repositories;
using DwitTech.WalletService.Core.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.AspNetCore.Http;
using DwitTech.WalletService.Data.Entities;
using System.Security.Claims;
using DwitTech.WalletService.Core.Exceptions;
using DwitTech.WalletService.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using DwitTech.WalletService.Core.Dtos;
using DwitTech.WalletService.Core.Models;

namespace DwitTech.WalletService.Core.Tests.Services
{
    public class UserWalletServiceTests
    {
        private readonly Mock<IWalletRepository> mockWalletRepo;
        private readonly Mock<IEmailService> mockEmailService;
        private readonly Mock<IHttpContextAccessor> mockHttpContextAccessor;
        private readonly Mock<ILogger<UserWalletService>> mockLogger;

        private readonly UserWalletService userWalletService;

        public UserWalletServiceTests()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "FROM_EMAIL", "your-from-email@example.com" }
                })
                .Build();

            mockWalletRepo = new Mock<IWalletRepository>();
            mockEmailService = new Mock<IEmailService>();
            mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockLogger = new Mock<ILogger<UserWalletService>>();

            userWalletService = new UserWalletService(
                mockWalletRepo.Object,
                mockLogger.Object,
                mockHttpContextAccessor.Object,
                mockEmailService.Object,
                configuration
            );
        }

        [Fact]
        public async Task CreateWallet_SendsEmailAndReturnsTrue_WhenSuccessful()
        {
            //Arrange
            var userId = 1;
            var validCurrencyCode = "USD";
            Currency currency = new Currency { Id = 1, Code = "USD", Name = "US Dollar", CreatedBy = "Admin" };


            mockWalletRepo.Setup(r => r.CheckCurrencyCode(validCurrencyCode)).Returns(true);
            mockWalletRepo.Setup(r => r.CheckWalletExists(userId, validCurrencyCode)).Returns(false);
            mockWalletRepo.Setup(r => r.GetCurrencyIdByCode(validCurrencyCode)).ReturnsAsync(currency);
            mockEmailService.Setup(x => x.SendMailAsync(It.IsAny<Email>(), false)).ReturnsAsync(true);


            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim("UserId", userId.ToString())
                }))
            };
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(httpContext);

            var createWalletRequest = new CreateWalletRequestDto
            {
                UserId = userId,
                CurrencyCode = validCurrencyCode,
                FullName = "John Doe",
                Email = "johndoe@example.com"
            };

            // Act
            var result = await userWalletService.CreateWallet(createWalletRequest);

            // Assert
            Assert.True(result);
            mockWalletRepo.Verify(r => r.CreateWallet(It.IsAny<Wallet>()), Times.Once);
            mockEmailService.Verify(x => x.SendMailAsync(It.IsAny<Email>(), false), Times.Once);
        }

        [Fact]
        public async Task CreateWallet_ThrowsArgumentNullException_WhenUserIdOrCurrencyCodeIsInvalid()
        {

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => userWalletService.CreateWallet(new CreateWalletRequestDto { UserId = 0, CurrencyCode = "USD" }));
            await Assert.ThrowsAsync<ArgumentNullException>(() => userWalletService.CreateWallet(new CreateWalletRequestDto { UserId = 1, CurrencyCode = null }));
            await Assert.ThrowsAsync<ArgumentNullException>(() => userWalletService.CreateWallet(new CreateWalletRequestDto { UserId = 1, CurrencyCode = "" }));
        }

        [Fact]
        public async Task ValidateCreateWalletRequest_ThrowsOperationCancelledException_WhenGetCurrencyIdByCodeThrowsSameException()
        {
            // Arrange
            var userId = 1;
            var validCurrencyCode = "USD";

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

            var createWalletRequest = new CreateWalletRequestDto
            {
                UserId = userId,
                CurrencyCode = validCurrencyCode,
                FullName = "John Doe",
                Email = "johndoe@example.com"
            };

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() => userWalletService.CreateWallet(createWalletRequest));
            mockWalletRepo.Verify(r => r.GetCurrencyIdByCode(validCurrencyCode), Times.Once);
        }

        [Fact]
        public async Task ValidateCreateWalletRequest_ThrowsArgumentOutOfRangeException_WhenCurrencyCodeIsInvalid()
        {
            // Arrange
            var userId = 1;
            var invalidCurrencyCode = "RRR";

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

            var createWalletRequest = new CreateWalletRequestDto
            {
                UserId = userId,
                CurrencyCode = invalidCurrencyCode,
                FullName = "John Doe",
                Email = "johndoe@example.com"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => userWalletService.CreateWallet(createWalletRequest));
            mockWalletRepo.Verify(r => r.GetCurrencyIdByCode(invalidCurrencyCode), Times.Never);
        }

        [Fact]
        public async Task ValidateCreateWalletRequest_ThrowsDuplicateRequestException_WhenWalletExists()
        {
            // Arrange
            var userId = 1;
            var validCurrencyCode = "USD";

            mockWalletRepo.Setup(r => r.CheckCurrencyCode(It.IsAny<string>())).Returns(true);
            mockWalletRepo.Setup(r => r.CheckWalletExists(It.IsAny<int>(), It.IsAny<string>())).Returns(true);

            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
            new Claim("UserId", userId.ToString())
        }))
            };
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(httpContext);

            var createWalletRequest = new CreateWalletRequestDto
            {
                UserId = userId,
                CurrencyCode = validCurrencyCode,
                FullName = "John Doe",
                Email = "johndoe@example.com"
            };

            // Act & Assert
            await Assert.ThrowsAsync<DuplicateRequestException>(() => userWalletService.CreateWallet(createWalletRequest));
            mockWalletRepo.Verify(r => r.GetCurrencyIdByCode(validCurrencyCode), Times.Never);
        }

        [Fact]
        public async Task CreateWallet_ThrowsNullReferenceException_WhenUserIdIsMissingFromToken()
        {
            // Arrange
            var userId = 1;
            var validCurrencyCode = "USD";

            // Set up HttpContextAccessor to return a context without UserId claim
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(new DefaultHttpContext());

            var createWalletRequest = new CreateWalletRequestDto
            {
                UserId = userId,
                CurrencyCode = validCurrencyCode,
                FullName = "John Doe",
                Email = "johndoe@example.com"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NullReferenceException>(() => userWalletService.CreateWallet(createWalletRequest));

            // Assert the exception message or anything else, if needed
            Assert.Contains("UserId is not present", exception.Message);
        }


        [Fact]
        public async Task CreateWallet_ThrowsDuplicateRequestException_WhenWalletExists()
        {
            // Arrange
            var userId = 2;
            var validCurrencyCode = "USD";

            mockWalletRepo.Setup(r => r.CheckCurrencyCode(It.IsAny<string>())).Returns(true);
            mockWalletRepo.Setup(r => r.CheckWalletExists(It.IsAny<int>(), It.IsAny<string>())).Returns(true);

            var claims = new[]
            {
                new Claim("UserId", userId.ToString())
            };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            };
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(httpContext);

            var createWalletRequest = new CreateWalletRequestDto
            {
                UserId = userId,
                CurrencyCode = validCurrencyCode,
                FullName = "John Doe",
                Email = "johndoe@example.com"
            };

            // Act & Assert
            await Assert.ThrowsAsync<DuplicateRequestException>(() => userWalletService.CreateWallet(createWalletRequest));
        }
    }
}