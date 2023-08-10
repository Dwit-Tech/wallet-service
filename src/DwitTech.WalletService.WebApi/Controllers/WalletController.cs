using AutoMapper;
using DwitTech.WalletService.Core.Dtos;
using DwitTech.WalletService.Core.Interfaces;
using DwitTech.WalletService.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DwitTech.WalletService.WebApi.Controllers
{
    public class WalletController : BaseController
    {
        private readonly IWalletService _walletService;
        private readonly ILogger<WalletController> _logger;
        private readonly IMapper _mapper;

        public WalletController(IWalletService walletService,
            ILogger<WalletController> logger,
            IMapper mapper)
        {
            _walletService = walletService;
            _logger = logger;
            _mapper = mapper;
        }


        [AllowAnonymous]
        [HttpGet("currencies")]
        public async Task<ActionResult<IEnumerable<Currency>>> GetAllCurrencies()
        {
            try
            {
                var currencies = await _walletService.GetAllCurrencies();
                return Ok(_mapper.Map<IEnumerable<CurrencyReadDto>>(currencies));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to retrieve currencies at this time");
                return NotFound("Currencies not found. Please try again");
            }
        }

        [HttpPost("createwallet")]
        public async Task<IActionResult> CreateWallet([FromBody] CreateWalletRequestDto createWalletRequest)
        {
            try
            {
                var result = await _walletService.CreateWallet(createWalletRequest);
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