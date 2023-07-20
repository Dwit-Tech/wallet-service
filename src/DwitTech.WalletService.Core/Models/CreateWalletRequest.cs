using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.WalletService.Core.Models
{
    public class CreateWalletRequest
    {
        public string UserName { get; set; }
        public string CurrencyType { get; set; }
    }
}
