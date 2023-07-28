﻿using DwitTech.WalletService.Core.Interfaces;
using DwitTech.WalletService.Core.Services;
using DwitTech.WalletService.Data.Context;
using DwitTech.WalletService.Data.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionsExtension
    {
        public static IServiceCollection AddDatabaseService(this IServiceCollection service, IConfiguration configuration)
        {

            string connectionString = configuration.GetConnectionString("WalletDbContext");
            connectionString = connectionString.Replace("{DBHost}", configuration["DB_HOSTNAME"]);
            connectionString = connectionString.Replace("{DBName}", configuration["DB_NAME"]);
            connectionString = connectionString.Replace("{DBUser}", configuration["DB_USERNAME"]);
            connectionString = connectionString.Replace("{DBPassword}", configuration["DB_PASSWORD"]);

            service.AddDbContext<WalletDbContext>(opt =>
            {
                opt.UseNpgsql(connectionString, c => c.CommandTimeout(120));
#if DEBUG
                opt.EnableSensitiveDataLogging();
                opt.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
#endif
            },
            contextLifetime: ServiceLifetime.Scoped,
            optionsLifetime: ServiceLifetime.Scoped);

            return service;
        }

        public static IServiceCollection AddServices(this IServiceCollection service, IConfiguration configuration)
        {

            service.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            service.AddScoped<IWalletService, UserWalletService>();
            service.AddScoped<IWalletRepository, WalletRepository>();

            return service;
        }

        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateAudience = false,
                    ValidIssuer = configuration["JWT_ISSUER"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT_KEY"]))
                };
            });
        }

    }
}
