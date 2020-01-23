using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JWT
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureServices(c =>
                    {
                        c.AddSingleton(typeof(WeatherForecast));
                    });
                })
                .ConfigureHostConfiguration(c =>
                {
                    c.AddEnvironmentVariables();
                })
                .ConfigureAppConfiguration(c =>
                {
                    c.AddEnvironmentVariables();
                })
                .ConfigureServices(c =>
                {

                });
    }
}
