using DwitTech.WalletService.Core.Dtos;
using DwitTech.WalletService.Core.Exceptions;
using DwitTech.WalletService.Core.Interfaces;
using DwitTech.WalletService.Core.Models;
using DwitTech.WalletService.Data.Entities;
using DwitTech.WalletService.Data.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Security.Claims;

namespace DwitTech.WalletService.Core.Services
{
    public class UserWalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly ILogger<UserWalletService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public UserWalletService(IWalletRepository walletRepository, ILogger<UserWalletService> logger, IHttpContextAccessor httpContextAccessor, IEmailService emailService, IConfiguration configuration)
        {
            _walletRepository = walletRepository;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
            _configuration = configuration;
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

        public async Task<string> GetTemplate(string templateName)
        {
            string trimmedTemplateName = templateName.Trim();
            var location = new FileInfo(Assembly.GetEntryAssembly().Location);

            string filePath = Path.Combine(location.DirectoryName, "Templates", trimmedTemplateName);

            var str = new StreamReader(filePath);
            var templateText = await str.ReadToEndAsync();
            str.Close();
            return templateText.ToString();
        }

        private async Task<bool> SendCreateWalletEmail(CreateWalletRequestDto createWalletRequest)
        {
            string templateText = await GetTemplate("WalletCreationEmail.html");
            templateText = templateText.Replace("{{Name}}", createWalletRequest.FullName);
            templateText = templateText.Replace("{{currencyCode}}", createWalletRequest.CurrencyCode);
            string body = templateText;
            const string subject = "Wallet Created";
            string fromEmail = _configuration["FROM_EMAIL"];
            var email = new Email { FromEmail = fromEmail, ToEmail = createWalletRequest.Email, Subject = subject, Body = body };
            var response = await _emailService.SendMailAsync(email);
            return response;
        }

        public async Task<bool> CreateWallet(CreateWalletRequestDto createWalletRequest)
        {
            if (createWalletRequest.UserId == 0 || string.IsNullOrWhiteSpace(createWalletRequest.CurrencyCode))
            {
                throw new ArgumentNullException("Username or currencyType cannot be null!");
            }

            var userIdFromToken = _httpContextAccessor.HttpContext!.User.FindFirst("UserId")?.Value;
            if (userIdFromToken == null)
            {
                throw new NullReferenceException("UserId is not present in this context.");
            }

            Currency validCurrency = await ValidateCreateWalletRequest(createWalletRequest.UserId, createWalletRequest.CurrencyCode);

            Wallet newWallet = new()
            {
                UserId = createWalletRequest.UserId,
                Currency = validCurrency,
                CreatedBy = userIdFromToken
            };

            _logger.LogTrace("Creating Wallet for user with ID {userId} was created successfully", createWalletRequest.UserId);
            await _walletRepository.CreateWallet(newWallet);
            _logger.LogInformation("Wallet for user with ID {userId} was created successfully", createWalletRequest.UserId);

            bool emailSent = await SendCreateWalletEmail(createWalletRequest);
            return emailSent;
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
