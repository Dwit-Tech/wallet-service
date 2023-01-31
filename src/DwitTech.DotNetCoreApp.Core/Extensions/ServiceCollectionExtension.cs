using DwitTech.DotNetCoreApp.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

                string connectionString = configuration.GetConnectionString("DefaultDbContext");
                connectionString = connectionString.Replace("{DBHost}", configuration["DB_HOSTNAME"]);
                connectionString = connectionString.Replace("{DBName}", configuration["DB_NAME"]);
                connectionString = connectionString.Replace("{DBUser}", configuration["DB_USERNAME"]);
                connectionString = connectionString.Replace("{DBPassword}", configuration["DB_PASSWORD"]);

                service.AddDbContext<DefaultDbContext>(opt =>
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

                return service;
            }
        }
    }
