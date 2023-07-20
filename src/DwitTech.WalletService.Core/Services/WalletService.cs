using DwitTech.WalletService.Core.Interfaces;
using DwitTech.WalletService.Data.Entities;
using DwitTech.WalletService.Data.Repositories;
using Microsoft.Extensions.Logging;

namespace DwitTech.WalletService.Core.Services
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly ILogger<WalletService> _logger;

        public WalletService(IWalletRepository walletRepository, ILogger<WalletService> logger)
        {
            _walletRepository = walletRepository;
            _logger = logger;
        }

        public async Task<bool> CreateWallet(string userName, string currencyType)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(currencyType))
            {
                throw new ArgumentException("Username or currencyType cannot be null!");
            }

            Wallet wallet = new()
            {
                Username = userName,
                Currency = currencyType
            };

            await _walletRepository.CreateWallet(wallet);
            _logger.LogInformation("Wallet for the user with ID {userName} was created successfully", userName);
            return true;
        }
    }
}
