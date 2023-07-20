using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using DwitTech.WalletService.Data.Entities;

namespace DwitTech.WalletService.Data.Context
{
    public class WalletDbContext : DbContext
    {
        public WalletDbContext(DbContextOptions<WalletDbContext> options) : base(options)
        {
        }

        public DbSet<Wallet> Wallets { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();
            builder.AddEntityConfigurations(assembly);
            base.OnModelCreating(builder);
        }
    }
}
