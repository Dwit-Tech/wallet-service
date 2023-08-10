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
        public DbSet<Currency> Currencies { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();
            builder.AddEntityConfigurations(assembly);
            base.OnModelCreating(builder);

            #region OwnedTypeSeed
            builder.Entity<Currency>().HasData(
                new Currency { Id = 1, Code = "NGN", Name = "Nigerian Naira", CreatedBy = "Seed Data" },
                new Currency { Id = 2, Code = "USD", Name = "US Dollar", CreatedBy = "Seed Data" },
                new Currency { Id = 3, Code = "GBP", Name = "British Pound", CreatedBy = "Seed Data" },
                new Currency { Id = 4, Code = "EUR", Name = "European Euro", CreatedBy = "Seed Data" },
                new Currency { Id = 5, Code = "CAD", Name = "Canadian Dollar", CreatedBy = "Seed Data" });
            #endregion
        }
    }
}