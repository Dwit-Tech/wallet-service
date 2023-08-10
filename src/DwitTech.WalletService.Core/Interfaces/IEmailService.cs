using DwitTech.WalletService.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.WalletService.Core.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendMailAsync(Email email, bool useHttp = false);
    }
}
