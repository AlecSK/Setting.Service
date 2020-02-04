using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Setting.Service.Application.Helpers;
using Setting.Service.Contract;
using Setting.Service.Contract.Interfaces;
using Setting.Service.DataAccess;
using Setting.Service.DataAccess.Moq;
using Setting.Service.Implementation;
using Xunit.Abstractions;

namespace Setting.Service.xUnitTests
{
    public abstract class TestRunnerBase
    {
        protected readonly IConfiguration Configuration;
        protected readonly ILoggerFactory LoggerFactory;
        protected readonly ServiceProvider ServiceProvider;
        protected readonly ITestOutputHelper Output;

        protected TestRunnerBase(ITestOutputHelper output)
        {
            Output = output;
            var profile = GetLaunchProfileName();

            Configuration = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.{profile}.json", optional: false)
                .Build();

            LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(Configuration.GetSection("Logging"));
                loggingBuilder.AddDebug();
                loggingBuilder.AddConsole();
            });

            var services = new ServiceCollection();
            services.AddSingleton(LoggerFactory);
            services.AddSingleton(Configuration);
            services.AddSingleton<IAppSettings, AppSettings>();
            services.AddSingleton<ICacheService<CacheableList<ConfigurationModule>>, CacheService<CacheableList<ConfigurationModule>>>();
            services.AddSingleton<ICacheService<CacheableList<ConfigurationSetting>>>(provider =>
            {
                var appSettings = provider.GetService<IAppSettings>();
                return new CacheService<CacheableList<ConfigurationSetting>>(new MemoryCacheOptions
                {
                    SizeLimit = appSettings.MaxSettingCountInCache
                });
            });

            services.AddScoped<ICachingRepository, CachingRepository>();

            string connection = Configuration.GetConnectionString("MasterDataDb");
            services.AddDbContext<MasterDataDbContext>(options =>
            {
                //options.UseSqlServer(connection);
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });

            ServiceProvider = services.BuildServiceProvider();
            SeedData.Initialize(ServiceProvider);
        }

        private static string GetLaunchProfileName()
        {
            var machineName = Environment.MachineName;
            if (machineName.Contains("DEV-01-PROC01", StringComparison.OrdinalIgnoreCase))
                return Environments.Staging;

            if (machineName.Contains("build-srv", StringComparison.OrdinalIgnoreCase))
                return Environments.Staging;

            return Environments.Development;
        }

        private static void SetEnvironmentVariables()
        {
            using (var file = File.OpenText("Properties\\launchSettings.json"))
            {
                var reader = new JsonTextReader(file);
                var jObject = JObject.Load(reader);

                var variables = jObject
                    .GetValue("profiles")
                    //select a proper profile here
                    .SelectMany(profiles => profiles.Children())
                    .SelectMany(profile => profile.Children<JProperty>())
                    .Where(prop => prop.Name == "environmentVariables")
                    .SelectMany(prop => prop.Value.Children<JProperty>())
                    .ToList();

                foreach (var variable in variables)
                {
                    Environment.SetEnvironmentVariable(variable.Name.Trim(), variable.Value.ToString().Trim());
                }
            }

            var envVariable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            Trace.WriteLine($"ASPNETCORE_ENVIRONMENT: '{envVariable}'");

        }
    }
}
