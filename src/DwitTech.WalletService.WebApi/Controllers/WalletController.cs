using DwitTech.WalletService.Core.Interfaces;
using DwitTech.WalletService.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace DwitTech.WalletService.WebApi.Controllers
{
    public class WalletController : BaseController
    {
        private readonly IWalletService _walletService;
        private readonly ILogger<WalletController> _logger;

        public WalletController(IWalletService walletService, ILogger<WalletController> logger)
        {
            _walletService = walletService;
            _logger = logger;
        }


        [HttpPost("createwallet")]
        public async Task<IActionResult> CreateWallet([FromBody] CreateWalletRequest walletDetails)
        {
            try
            {
                var result = await _walletService.CreateWallet(walletDetails.UserName, walletDetails.CurrencyType);
                if (result)
                    return new CreatedResult("Wallet", null);
                return BadRequest("Unable to create wallet. Please try again later");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to create wallet");
                return BadRequest("Unable to create wallet. Please check data and try again");
            }
        }
    }
}
