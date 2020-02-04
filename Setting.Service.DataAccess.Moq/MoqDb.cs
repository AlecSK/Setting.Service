using System;
using System.Collections.Generic;
using Setting.Service.Common;

namespace Setting.Service.DataAccess.Moq
{
    public static class MoqDb
    {
        private const int SettingServiceModuleId = 1;
        private static bool _isInitialized;
        private static readonly object Locker = new object();

        public static void InitializeDbForTests(MasterDataDbContext db)
        {
            lock (Locker)
            {
                if (_isInitialized)
                    return;

                db.Modules.AddRange(GetSeedingModules());
                db.Settings.AddRange(GetSeedingSettings());
                db.SaveChanges();
                _isInitialized = true;
            }

        }

        public static void ReinitializeDbForTests(MasterDataDbContext db)
        {
            db.Modules.RemoveRange(db.Modules);
            db.Settings.RemoveRange(db.Settings);
            InitializeDbForTests(db);
        }

        private static IEnumerable<ConfigurationModule> GetSeedingModules()
        {
            var rnd = new Random();

            yield return new ConfigurationModule
            {
                Id = SettingServiceModuleId,
                Name = "SettingService",
                SystemName = "Setting.Service",
                ModuleCacheLifeTimeInMinutes = null,
                Description = "Сервис настроек"
            };

            for (int i = 2; i <= 6; i++)
            {
                yield return new ConfigurationModule
                {
                    Id = i,
                    Name = "Module" + rnd.Next(100),
                    SystemName = "Module" + rnd.Next(100),
                    ModuleCacheLifeTimeInMinutes = rnd.Next(100)
                };
            }
        }

        private static IEnumerable<ConfigurationSetting> GetSeedingSettings()
        {
            var rnd = new Random();

            // настройки модуля SettingService
            yield return new ConfigurationSetting
            {
                ModuleId = SettingServiceModuleId,
                Name = "DefaultCacheLifeTimeInMinutes",
                Value = "60"
            };
            yield return new ConfigurationSetting
            {
                ModuleId = SettingServiceModuleId,
                Name = "MaxSettingCountInCache",
                Value = "5000"
            };
            yield return new ConfigurationSetting
            {
                ModuleId = SettingServiceModuleId,
                Name = AppConstants.UserCredentials,
                Value = "user1:password1;user2:password2"
            };

            // для модуля ModuleId=2 настройки кэширования беруться из SettingService.DefaultCacheLifeTimeInMinutes
            for (int j = 1; j < 10; j++)
            {
                yield return new ConfigurationSetting
                {
                    ModuleId = 2,
                    Name = "Setting" + rnd.Next(10000),
                    Value = "Value" + rnd.Next(10000),
                    SettingCacheLifeTimeInMinutes = null,
                };
            }


            // для остальных модулей настройки кэширования устанавливаются случайно
            for (int moduleId = 3; moduleId <= 6; moduleId++)
            {
                int settingsCount = rnd.Next(1, 30);
                for (int j = 1; j < settingsCount; j++)
                {
                    yield return new ConfigurationSetting
                    {
                        ModuleId = moduleId,
                        Name = "Setting" + rnd.Next(10000),
                        Value = "Value" + rnd.Next(10000),
                        SettingCacheLifeTimeInMinutes = rnd.Next(0, 5),
                    };
                }
            }

            // Real data
            yield return new ConfigurationSetting
            {
                ModuleId = 3,
                Name = "MsmqNameDefault" ,
                Value = "MS Message Queue" ,
                SettingCacheLifeTimeInMinutes = rnd.Next(0, 60),
            };

            yield return new ConfigurationSetting
            {
                ModuleId = 4,
                Name = "TaskServiceName" ,
                Value = "Task Checks " ,
                SettingCacheLifeTimeInMinutes = rnd.Next(0, 60),
            };

            yield return new ConfigurationSetting
            {
                ModuleId = 5,
                Name = "TaskServiceName" ,
                Value = "Task Mediator " ,
                SettingCacheLifeTimeInMinutes = rnd.Next(0, 60),
            };
        }
    }
}
