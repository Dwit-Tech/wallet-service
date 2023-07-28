

using AutoMapper;
using DwitTech.WalletService.Core.Interfaces;
using DwitTech.WalletService.Core.Models;
using DwitTech.WalletService.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace DwitTech.WalletService.WebApi.Tests.Controllers
{
    public class WalletControllerTests
    {
        [Fact]
        public async Task Check_CreateWalletReturnsCreatedResult_WhenSuccessful()
        {
            //Arrange
            var walletDetails = new CreateWalletRequest { UserId = 1, CurrencyCode = "NGN" };
            var mockWalletService = new Mock<IWalletService>();
            mockWalletService.Setup(ws => ws.CreateWallet(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(true);

            var mockLogger = new Mock<ILogger<WalletController>>();
            var mockMapper = new Mock<IMapper>();

            var walletContoller = new WalletController(mockWalletService.Object, mockLogger.Object, mockMapper.Object);

            //Act
            var result = await walletContoller.CreateWallet(walletDetails);

            //Assert
            mockWalletService.Verify(ws => ws.CreateWallet(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
            Assert.True(result is CreatedResult);
        }

        [Fact]
        public async Task Check_CreateWalletReturnsBadRequest_WhenExceptionIsCaught()
        {
            //Arrange
            var walletDetails = new CreateWalletRequest { UserId = 1, CurrencyCode = "NGN" };
            var mockWalletService = new Mock<IWalletService>();
            mockWalletService.Setup(ws => ws.CreateWallet(It.IsAny<int>(), It.IsAny<string>())).Throws(new ArgumentOutOfRangeException());

            var mockLogger = new Mock<ILogger<WalletController>>();
            var mockMapper = new Mock<IMapper>();

            var walletContoller = new WalletController(mockWalletService.Object, mockLogger.Object, mockMapper.Object);

            //Act
            var result = await walletContoller.CreateWallet(walletDetails);

            //Assert
            mockWalletService.Verify(ws => ws.CreateWallet(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
            Assert.True(result is BadRequestObjectResult);
        }
    }
}
