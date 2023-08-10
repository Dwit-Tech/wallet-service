using AutoMapper;
using DwitTech.WalletService.Core.Dtos;
using DwitTech.WalletService.Core.Interfaces;
using DwitTech.WalletService.Core.Models;
using DwitTech.WalletService.Data.Entities;
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
            // Arrange
            var walletDetails = new CreateWalletRequestDto { UserId = 1, CurrencyCode = "NGN" };
            var mockWalletService = new Mock<IWalletService>();

            // Set up the mock to return true (successful creation)
            mockWalletService.Setup(ws => ws.CreateWallet(It.IsAny<CreateWalletRequestDto>())).ReturnsAsync(true);

            var mockLogger = new Mock<ILogger<WalletController>>();
            var mockMapper = new Mock<IMapper>();

            var walletContoller = new WalletController(mockWalletService.Object, mockLogger.Object, mockMapper.Object);

            // Act
            var result = await walletContoller.CreateWallet(walletDetails);

            // Assert
            mockWalletService.Verify(ws => ws.CreateWallet(It.IsAny<CreateWalletRequestDto>()), Times.Once);
            Assert.IsType<CreatedResult>(result);
        }

        [Fact]
        public async Task Check_CreateWalletReturnsBadRequest_WhenExceptionIsCaught()
        {
            //Arrange
            var walletDetails = new CreateWalletRequestDto { UserId = 1, CurrencyCode = "NGN" };
            var mockWalletService = new Mock<IWalletService>();
            mockWalletService.Setup(ws => ws.CreateWallet(It.IsAny<CreateWalletRequestDto>())).Throws(new ArgumentOutOfRangeException());

            var mockLogger = new Mock<ILogger<WalletController>>();
            var mockMapper = new Mock<IMapper>();

            var walletContoller = new WalletController(mockWalletService.Object, mockLogger.Object, mockMapper.Object);

            //Act
            var result = await walletContoller.CreateWallet(walletDetails);

            //Assert
            mockWalletService.Verify(ws => ws.CreateWallet(It.IsAny<CreateWalletRequestDto>()), Times.Once);
            Assert.True(result is BadRequestObjectResult);
        }
    }
}