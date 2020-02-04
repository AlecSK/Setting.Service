using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Setting.Service.Common;
using Setting.Service.Contract;
using Setting.Service.Contract.Interfaces;
using Setting.Service.DataAccess;

namespace Setting.Service.Application.Helpers
{
    public class AppSettings : IAppSettings
    {
        private readonly ILogger<AppSettings> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _scopeFactory;

        public int DefaultCacheLifeTimeInMinutes { get; set; } = 60;
        public int MaxSettingCountInCache { get; set; } = 5000;

        public AppSettings(ILoggerFactory loggerFactory, IConfiguration configuration, IServiceProvider scopeFactory)
        {
            _logger = loggerFactory.CreateLogger<AppSettings>();
            _configuration = configuration;
            _scopeFactory = scopeFactory;
            LoadSettings();
        }


        private void LoadSettings()
        {
            using (var scope = _scopeFactory.CreateScope()){
                var db = scope.ServiceProvider.GetRequiredService<MasterDataDbContext>();

                try
                {
                    var settingServiceModule = db.Modules.FirstOrDefault(t => t.Name == AppConstants.SettingServiceModuleName);
                    // ReSharper disable PossibleNullReferenceException
                    // ReSharper disable once AssignNullToNotNullAttribute
                    db.Entry(settingServiceModule).Collection(t => t.Settings).Load();

                    var defaultCacheLifeTimeInMinutes = settingServiceModule.Settings.FirstOrDefault(t => t.Name == AppConstants.DefaultCacheLifeTimeInMinutes);
                    DefaultCacheLifeTimeInMinutes = int.Parse(defaultCacheLifeTimeInMinutes.Value);

                    var maxSettingCountInCache = settingServiceModule.Settings.FirstOrDefault(t => t.Name == AppConstants.MaxSettingCountInCache);
                    MaxSettingCountInCache = int.Parse(maxSettingCountInCache.Value);

                    // ReSharper restore PossibleNullReferenceException
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Не удалось загрузить настройки сервиса из БД. Будут использованы стандартные параметры.");
                }

            }
        }


    }
}