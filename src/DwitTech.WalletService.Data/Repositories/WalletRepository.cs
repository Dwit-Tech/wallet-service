
using DwitTech.WalletService.Data.Context;
using DwitTech.WalletService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DwitTech.WalletService.Data.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly WalletDbContext _dbContext;

        public WalletRepository(WalletDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool CheckCurrencyCode(string currencyCode)
        {
            return _dbContext.Currencies.Any(c => c.Code == currencyCode);
        }

        public bool CheckDuplicateWallet(string userName, string currencyCode)
        {
           return _dbContext.Wallets.Any(w => w.Username == userName && w.Currency.Code == currencyCode);
        }

        public async Task CreateWallet(Wallet wallet)
        {
            await _dbContext.Wallets.AddAsync(wallet);
            _dbContext.Attach(wallet.Currency);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Currency>> GetAllCurrencies()
        {
            return await _dbContext.Currencies.ToListAsync();
        }

        public async Task<Currency> GetCurrencyIdByCode(string currencyCode)
        {
            return await _dbContext.Currencies.Where(c => c.Code == currencyCode).FirstAsync();
        }
    }
}
