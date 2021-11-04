using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            //by using the using keyword we are going to dispose the scope when the method is done
            using var scope = host.Services.CreateScope();

            var services  = scope.ServiceProvider;
            try{
                // we have added DataContext as a service.
                // we are using the service locator pattern
                var context = services.GetRequiredService<DataContext>();
                await context.Database.MigrateAsync();// this is going to create the databse if we don´t have it
                await Seed.SeedData(context);
            }catch (Exception ex){
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occured during the migration");
            }
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
