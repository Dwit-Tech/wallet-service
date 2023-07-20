﻿using DwitTech.WalletService.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.WalletService.Data.Repositories
{
    public interface IWalletRepository
    {
        Task CreateWallet(Wallet wallet);
    }
}
