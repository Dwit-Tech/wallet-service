using DwitTech.DotNetCoreApp.Data.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Microsoft.Extensions.DependencyInjection
{
    public static class AppBuilderExtension
    {
        public static IApplicationBuilder SetupMigrations(this IApplicationBuilder app, IServiceProvider service, IConfiguration configuration)
        {
            var logger = service.GetService<ILogger<DefaultDbContext>>();

            try
            {
                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var context = serviceScope.ServiceProvider.GetService<DefaultDbContext>();
                    //context.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }

            return app;
        }
    }
}
