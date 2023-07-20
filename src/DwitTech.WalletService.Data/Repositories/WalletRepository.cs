
using DwitTech.WalletService.Data.Context;
using DwitTech.WalletService.Data.Entities;

namespace DwitTech.WalletService.Data.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly WalletDbContext _dbContext;

        public WalletRepository(WalletDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateWallet(Wallet wallet)
        {
            await _dbContext.Wallets.AddAsync(wallet);
            await _dbContext.SaveChangesAsync();
        }
    }
}
