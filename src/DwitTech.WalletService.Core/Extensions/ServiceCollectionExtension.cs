using Confluent.Kafka;
using DwitTech.WalletService.Core.Events;
using DwitTech.WalletService.Core.Interfaces;
using DwitTech.WalletService.Core.Services;
using DwitTech.WalletService.Data.Context;
using DwitTech.WalletService.Data.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
            service.AddScoped<IEmailService, EmailService>();
            service.AddScoped<IEmailEventPublisher, EmailEventPublisher>();
            service.AddHttpClient();
            service.AddSingleton<IProducer<string, string>>(provider =>
            {
                var producerConfig = new ProducerConfig
                {
                    BootstrapServers = configuration["MESSAGE_BROKER_BOOTSTRAP_SERVERS"],
                    ClientId = configuration["MESSAGE_BROKER_CLIENT_ID"],

                    SecurityProtocol = Enum.Parse<SecurityProtocol>(configuration["KAFKA_SECURITY_PROTOCOL"])
                };

                switch (producerConfig.SecurityProtocol)
                {
                    case null:
                    case SecurityProtocol.Plaintext:
                        break;
                    case SecurityProtocol.Ssl:
                        break;

                    case SecurityProtocol.SaslSsl:
                        producerConfig.SaslMechanism = Enum.Parse<SaslMechanism>(configuration["KAFKA_SASL_MECHANISM"]);
                        producerConfig.SaslUsername = configuration["KAFKA_SASL_USERNAME"];
                        producerConfig.SaslPassword = configuration["KAFKA_SASL_PASSWORD"];
                        break;

                    case SecurityProtocol.SaslPlaintext:
                        throw new NotImplementedException($"Security Protocol {producerConfig.SecurityProtocol} is not implemented");

                }

                return new ProducerBuilder<string, string>(producerConfig).Build();
            });

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
                options.Authority = configuration["JWT:Authority"];
                options.Audience = configuration["JWT:Audience"];

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateAudience = false,
                    ValidAudiences = new List<string> { configuration["JWT:Audience"] },
                    ValidIssuer = configuration["JWT:JWT_ISSUER"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:JWT_KEY"]))
                };
            });
        }

    }
}
