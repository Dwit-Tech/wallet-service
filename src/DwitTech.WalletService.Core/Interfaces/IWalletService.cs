using DwitTech.WalletService.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.WalletService.Core.Interfaces
{
    public interface IWalletService
    {
        Task<IEnumerable<Currency>> GetAllCurrencies();
        Task<bool> CreateWallet(int userId, string currencyType);
    }
}
