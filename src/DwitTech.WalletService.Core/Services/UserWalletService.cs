using DwitTech.WalletService.Core.Exceptions;
using DwitTech.WalletService.Core.Interfaces;
using DwitTech.WalletService.Data.Entities;
using DwitTech.WalletService.Data.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace DwitTech.WalletService.Core.Services
{
    public class UserWalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly ILogger<UserWalletService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserWalletService(IWalletRepository walletRepository, ILogger<UserWalletService> logger, IHttpContextAccessor httpContextAccessor)
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

        public async Task<bool> CreateWallet(int userId, string currencyCode)
        {
            if (userId == 0 || string.IsNullOrWhiteSpace(currencyCode))
            {
                throw new ArgumentNullException("Username or currencyType cannot be null!");
            }

            var userIdFromToken = _httpContextAccessor.HttpContext!.User.FindFirst("UserId")?.Value;
            if (userIdFromToken == null)
            {
                throw new NullReferenceException("UserId is not present in this context.");
            }

            Currency validCurrency = await ValidateCreateWalletRequest(userId, currencyCode);

            Wallet newWallet = new()
            {
                UserId = userId,
                Currency = validCurrency,
                CreatedBy = userIdFromToken
            };

            _logger.LogTrace("Creating Wallet for user with ID {userId} was created successfully", userId);
            await _walletRepository.CreateWallet(newWallet);
            _logger.LogInformation("Wallet for user with ID {userId} was created successfully", userId);

            return true;
        }

        private async Task<Currency> ValidateCreateWalletRequest(int userId, string currencyCode)
        {
            try
            {
                if (!_walletRepository.CheckCurrencyCode(currencyCode))
                {
                    _logger.LogError("ArgumentOutOfRangeException thrown because currency code does not exist!");
                    throw new ArgumentOutOfRangeException("Currency code does not exist!", nameof(currencyCode));
                }

                if (_walletRepository.CheckWalletExists(userId, currencyCode))
                {
                    _logger.LogError("DuplicateEntryException thrown because user already has a wallet in this currency!");
                    throw new DuplicateRequestException($"Wallet already exists for user with Id {userId}");
                }

                return await _walletRepository.GetCurrencyIdByCode(currencyCode);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError("Error retrieving currency from db");
                throw ex;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Currency with code {currencyCode} does not exist!");
                throw ex;
            }
        }
    }
}
