using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using BinanceBotDb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BinanceBotWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using var scope = host.Services.CreateScope();
            var context = scope.ServiceProvider.GetService<IBinanceBotDbContext>();
            
            if(context is null)
                Console.WriteLine("Error! EF context object is null. Database migration was not done.");
            
            context?.Database.Migrate();
            host.Run();
        }
        
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}