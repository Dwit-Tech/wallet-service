

using DwitTech.WalletService.Data.Context;
using DwitTech.WalletService.Data.Entities;
using DwitTech.WalletService.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DwitTech.WalletService.Data.Tests.Repositories
{
    public class WalletRepositoryTests : IDisposable
    {
        [Fact]
        public async Task Check_CreateWallet_CreatesWallet_WhenWalletIsValid()
        {
            //Arrange
            var wallet = new Wallet
            {
                UserId = 1,
                Currency = new Currency { Id = 1, Code = "NGN", Name = "Nigerian Naira", CreatedBy = "Magician" },
                CreatedBy = "1"
            };

            var options = new DbContextOptionsBuilder<WalletDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            using(var walletDbContext = new WalletDbContext(options))
            {
                var walletRepo = new WalletRepository(walletDbContext);

                //Act
                await walletRepo.CreateWallet(wallet);

                //Assert
                var createdWallet = await walletDbContext.Wallets.FindAsync(wallet.UserId);
                Assert.NotNull(createdWallet);
                Assert.Equal(wallet, createdWallet);
            }
        }

        [InlineData(1, "NGN", true)]
        [InlineData(1, "RED", false)]
        [InlineData(2, "NGN", false)]
        [InlineData(2, "RED", false)]
        [Theory]
        public async Task Check_CheckWalletExists_ReturnsBooleanValue(int userId, string currencyCode, bool expected)
        {
            //Arrange
            var wallet = new Wallet
            {
                UserId = 1,
                Currency = new Currency { Id = 1, Code = "NGN", Name = "Nigerian Naira", CreatedBy = "Magician" },
                CreatedBy = "1"
            };

            var options = new DbContextOptionsBuilder<WalletDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            using (var walletDbContext = new WalletDbContext(options))
            {
                var walletRepo = new WalletRepository(walletDbContext);

                //Act
                await walletDbContext.Currencies.AddAsync(wallet.Currency);
                await walletDbContext.SaveChangesAsync();

                await walletRepo.CreateWallet(wallet);

                var result = walletRepo.CheckWalletExists(userId, currencyCode);

                //Assert
                Assert.Equal(result, expected);
            }
        }

        [InlineData("NGN", true)]
        [InlineData("RED", false)]
        [Theory]
        public async Task Check_CheckCurrencyCode_ReturnsBooleanValue(string currencyCode, bool expected)
        {
            //Arrange           
            var currency = new Currency { Id = 1, Code = "NGN", Name = "Nigerian Naira", CreatedBy = "Magician" };

            var options = new DbContextOptionsBuilder<WalletDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            using (var walletDbContext = new WalletDbContext(options))
            {
                var walletRepo = new WalletRepository(walletDbContext);

                //Act
                await walletDbContext.Currencies.AddAsync(currency);
                await walletDbContext.SaveChangesAsync();

                var result = walletRepo.CheckCurrencyCode(currencyCode);

                //Assert
                Assert.Equal(result, expected);
            }
        }

        [Fact]
        public async Task Check_GetCurrencyIdByCode_ReturnsCurrencyWhenCurrencyCodeIsValid()
        {
            //Arrange           
            var currency = new Currency { Id = 1, Code = "NGN", Name = "Nigerian Naira", CreatedBy = "Magician" };

            var options = new DbContextOptionsBuilder<WalletDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            using (var walletDbContext = new WalletDbContext(options))
            {
                var walletRepo = new WalletRepository(walletDbContext);

                //Act
                await walletDbContext.Currencies.AddAsync(currency);
                await walletDbContext.SaveChangesAsync();

                var result = await walletRepo.GetCurrencyIdByCode(currency.Code);

                //Assert
                Assert.Equal(result, currency);
            }
        }
        
        [Fact]
        public async Task Check_GetCurrencyIdByCode_ThrowsInvalidOperationExceptionWhenCurrencyCodeIsInvalid()
        {
            //Arrange
            var invalidCurrencyCode = "RED";
            var currency = new Currency { Id = 1, Code = "NGN", Name = "Nigerian Naira", CreatedBy = "Magician" };

            var options = new DbContextOptionsBuilder<WalletDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            using (var walletDbContext = new WalletDbContext(options))
            {
                var walletRepo = new WalletRepository(walletDbContext);

                //Act
                await walletDbContext.Currencies.AddAsync(currency);
                await walletDbContext.SaveChangesAsync();

                async Task<Currency> act() => await walletRepo.GetCurrencyIdByCode(invalidCurrencyCode);

                //Assert
                await Assert.ThrowsAsync<InvalidOperationException>(act);
            }
        }

        [Fact]
        public async Task Check_GetAllCurrencies_ReturnsAllCurrencies()
        {
            //Arrange
            var listOfCurrencies = new List<Currency> { new Currency { Id = 1, Code = "NGN", Name = "Nigerian Naira", CreatedBy = "Magician" },
            new Currency { Id = 2, Code = "USD", Name = "US Dollar", CreatedBy = "SomeoneELse"} };

            var options = new DbContextOptionsBuilder<WalletDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            using (var walletDbContext = new WalletDbContext(options))
            {
                var walletRepo = new WalletRepository(walletDbContext);

                await walletDbContext.Currencies.AddAsync(listOfCurrencies[0]);
                await walletDbContext.Currencies.AddAsync(listOfCurrencies[1]);
                await walletDbContext.SaveChangesAsync();

                //Act
                var result = await walletRepo.GetAllCurrencies();

                //Assert
                Assert.Equal(result, listOfCurrencies);
            }
        }

            public void Dispose()
        {
        }
    }
}
