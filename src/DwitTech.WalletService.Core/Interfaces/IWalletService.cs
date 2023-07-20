using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.WalletService.Core.Interfaces
{
    public interface IWalletService
    {
        Task<bool> CreateWallet(string userName, string currencyType);
    }
}
