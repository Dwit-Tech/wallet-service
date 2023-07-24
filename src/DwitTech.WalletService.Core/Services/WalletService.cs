using DwitTech.WalletService.Core.Exceptions;
using DwitTech.WalletService.Core.Interfaces;
using DwitTech.WalletService.Data.Entities;
using DwitTech.WalletService.Data.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace DwitTech.WalletService.Core.Services
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly ILogger<WalletService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WalletService(IWalletRepository walletRepository, ILogger<WalletService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _walletRepository = walletRepository;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<Currency>> GetAllCurrencies()
        {
            try
            {
                return await _walletRepository.GetAllCurrencies();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> CreateWallet(string userName, string currencyCode)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(currencyCode))
            {
                throw new ArgumentException("Username or currencyType cannot be null!");
            }

            var userIdFromToken = _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.Email)?.Value;
            if (userIdFromToken == null)
            {
                throw new NullReferenceException("Email is not present in this context.");
            }

            Currency validCurrency = await ValidateCreateWalletRequest(userName, currencyCode);

            Wallet newWallet = new()
            {
                Username = userName,
                Currency = validCurrency,
                CreatedBy = userIdFromToken
            };

            _logger.LogTrace("Creating Wallet for user with ID {userName} was created successfully", userName);
            await _walletRepository.CreateWallet(newWallet);
            _logger.LogInformation("Wallet for user with ID {userName} was created successfully", userName);

            return true;
        }

        private async Task<Currency> ValidateCreateWalletRequest(string userName, string currencyCode)
        {
            if (!_walletRepository.CheckCurrencyCode(currencyCode))
            {
                _logger.LogInformation("ArgumentOutOfRangeException thrown because currency code does not exist!");
                throw new ArgumentOutOfRangeException("Currency code does not exist!", nameof(currencyCode));
            }

            if (_walletRepository.CheckDuplicateWallet(userName, currencyCode))
            {
                _logger.LogInformation("DuplicateEntryException thrown because user already has a wallet in this currency!");
                throw new DuplicateRequestException("Wallet already exists for user {userName}");
            }

            return await _walletRepository.GetCurrencyIdByCode(currencyCode);
        }
    }
}
