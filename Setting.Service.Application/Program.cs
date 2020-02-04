using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using Setting.Service.Application.Utils;

namespace Setting.Service.Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var nLogConfigurationFile = "NLog.config";
            var envVariable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (envVariable == Environments.Development)
                nLogConfigurationFile = "NLog.Development.config";

            var logger = NLogBuilder.ConfigureNLog(nLogConfigurationFile).GetCurrentClassLogger();
            try
            {
                logger.Warn($"init Main. ASPNETCORE_ENVIRONMENT: '{envVariable}'");
                CreateHostBuilder(args).Build().Seed().Run();
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(logging =>
                {
                    var envVariable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                    var configuration = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json")
                        .AddJsonFile($"appsettings.{envVariable}.json", optional: true)
                        .Build();

                    logging.ClearProviders();
                    logging.AddConfiguration(configuration.GetSection("Logging"));
                    //logging.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog();

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureLogging(logging =>
                {
                    var envVariable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                    var configuration = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json")
                        .AddJsonFile($"appsettings.{envVariable}.json", optional: true)
                        .Build();

                    logging.ClearProviders();
                    logging.AddConfiguration(configuration.GetSection("Logging"));
                    //logging.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog();
    }
}
