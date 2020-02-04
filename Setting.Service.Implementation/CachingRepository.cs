using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Setting.Service.Contract;
using Setting.Service.Contract.DtoModels;
using Setting.Service.Contract.Interfaces;
using Setting.Service.DataAccess;

namespace Setting.Service.Implementation
{
    public sealed class CachingRepository : ICachingRepository
    {
        private readonly ILogger<CachingRepository> _logger;
        private readonly IAppSettings _appSettings;
        private readonly MasterDataDbContext _db;
        private readonly ICacheService<CacheableList<ConfigurationModule>> _modulesCache;
        private readonly ICacheService<CacheableList<ConfigurationSetting>> _settingsCache;


        public CachingRepository(
            ILoggerFactory loggerFactory,
            IAppSettings appSettings,
            MasterDataDbContext db,
            ICacheService<CacheableList<ConfigurationModule>> modulesCache,
            ICacheService<CacheableList<ConfigurationSetting>> settingsCache)
        {
            _logger = loggerFactory.CreateLogger<CachingRepository>();
            _appSettings = appSettings;
            _db = db;
            _modulesCache = modulesCache;
            _settingsCache = settingsCache;
        }


        public void ResetCache()
        {
            _settingsCache.ResetCache(new MemoryCacheOptions
            {
                SizeLimit = _appSettings.MaxSettingCountInCache
            });
            _modulesCache.ResetCache();
            _logger.LogDebug("{0}   Кэш очищен.", "ResetCache");
        }


        public List<DtoConfigurationSetting> GetDtoSettingValuesByModuleIdWithCache(int moduleId, bool refreshCache = false)
        {
            var moduleSettings = GetSettingValuesByModuleIdWithCache(moduleId, refreshCache);
            return moduleSettings.Select(t => new DtoConfigurationSetting { Name = t.Name, Value = t.Value }).ToList();
        }


        public string Get1SettingValueByModuleId(int moduleId, string settingName)
        {
            if (_settingsCache.TryGetCacheValue(moduleId, out var cachedSettings))
            {
                var cached1Setting = cachedSettings.FirstOrDefault(t => t.Name == settingName);
                if (cached1Setting != null)
                {
                    _logger.LogDebug("{0}   {1} Данные найдены в кэше. Проверяем политики кэширования.", "Get1SettingValueByModuleId", moduleId);
                    if (!cached1Setting.IsExpired(_appSettings.DefaultCacheLifeTimeInMinutes))
                    {
                        _logger.LogDebug("{0}   {1} Возвращаем данные из кэша", "Get1SettingValueByModuleId", moduleId);
                        return cached1Setting.Value;
                    }
                }
            }

            var moduleSettings = GetModuleSettingsAndStoreInCacheIfPossible(moduleId);
            var module1Setting = moduleSettings.FirstOrDefault(t => t.Name == settingName);
            if (module1Setting != null)
            {
                _logger.LogDebug("{0}   {1} Возвращаем данные из БД", "Get1SettingValueByModuleId", moduleId);
                return module1Setting.Value;
            }

            var globalSettings = GetOrStoreGlobalSettings();
            var global1Setting = globalSettings.FirstOrDefault(t => t.Name == settingName);
            if (global1Setting != null)
            {
                _logger.LogDebug("{0}   {1} Возвращаем данные из общих настроек.", "Get1SettingValueByModuleId", moduleId);
                return global1Setting.Value;
            }

            return null;
        }


        public List<DtoConfigurationModule> GetDtoModulesWithCache()
        {
            return GetModulesWithCache().Select(t => new DtoConfigurationModule { SystemName = t.SystemName, Id = t.Id }).ToList();
        }


        private List<ConfigurationSetting> GetSettingValuesByModuleIdWithCache(int moduleId, bool refreshCache)
        {
            if (!refreshCache)
            {
                if (_settingsCache.TryGetCacheValue(moduleId, out var cachedSettings))
                {
                    _logger.LogDebug("{0}   {1} Данные найдены в кэше. Проверяем политики кэширования.", "GetSettingValuesByModuleIdWithCache", moduleId);
                    if (!cachedSettings.Any(t => t.IsExpired(cachedSettings.CacheEntryLifeTimeInMinutes)))
                    {
                        _logger.LogDebug("{0}   {1} Возвращаем данные из кэша", "GetSettingValuesByModuleIdWithCache", moduleId);
                        return cachedSettings;
                    }
                }
            }

            var moduleSettings = GetModuleSettingsAndStoreInCacheIfPossible(moduleId);
            return moduleSettings;
        }

        private List<ConfigurationSetting> GetModuleSettingsAndStoreInCacheIfPossible(int moduleId)
        {
            _logger.LogDebug("{0}   {1} Загружаем настройки кэширования для конкретного модуля", "GetModuleSettingsAndStoreInCacheIfPossible", moduleId);
            var moduleLifeTimeInMinutes = _appSettings.DefaultCacheLifeTimeInMinutes;
            var module = _db.Modules.FirstOrDefault(t => t.Id == moduleId);
            if (module?.ModuleCacheLifeTimeInMinutes != null)
                if (module.ModuleCacheLifeTimeInMinutes == 0)
                    return _db.Settings.Where(t => t.ModuleId == moduleId).ToList();
                else
                    moduleLifeTimeInMinutes = module.ModuleCacheLifeTimeInMinutes.Value;

            var settingsFromDb = _settingsCache.GetAndStore(moduleId, () =>
            {
                var settings = _db.Settings.Where(t => t.ModuleId == moduleId);
                var cacheableSettings = new CacheableList<ConfigurationSetting>(settings, moduleLifeTimeInMinutes);
                return cacheableSettings;
            });
            _logger.LogDebug("{0}   {1} Возвращаем найденный в БД результат.", "GetModuleSettingsAndStoreInCacheIfPossible", moduleId);
            return settingsFromDb;
        }

        private CacheableList<ConfigurationSetting> GetOrStoreGlobalSettings()
        {
            var moduleId = 0;
            if (_settingsCache.TryGetCacheValue(moduleId, out var cachedSettings))
            {
                _logger.LogDebug("{0}   {1} Данные найдены в кэше. Проверяем политики кэширования.", "GetOrStoreGlobalSettings", moduleId);
                if (!cachedSettings.Any(t => t.IsExpired(cachedSettings.CacheEntryLifeTimeInMinutes)))
                {
                    _logger.LogDebug("{0}   {1} Возвращаем данные из кэша", "GetOrStoreGlobalSettings", moduleId);
                    return cachedSettings;
                }
            }

            var settingsFromDb = _settingsCache.GetAndStore(moduleId, () =>
            {
                var settings = _db.Settings.Where(t => t.ModuleId == null);
                var cacheableSettings = new CacheableList<ConfigurationSetting>(settings, _appSettings.DefaultCacheLifeTimeInMinutes);
                return cacheableSettings;
            });
            _logger.LogDebug("{0}   {1} Возвращаем найденный в БД результат.", "GetOrStoreGlobalSettings", moduleId);
            return settingsFromDb;
        }

        private CacheableList<ConfigurationModule> GetModulesWithCache()
        {
            if (_modulesCache.TryGetCacheValue(0, out var cached))
                if (!cached.IsExpired())
                {
                    _logger.LogDebug("{0}   Модули загружены из кэша.", "GetModulesWithCache");
                    return cached;
                }

            var modules = _modulesCache.GetAndStore(0, () =>
            {
                return new CacheableList<ConfigurationModule>(_db.Modules.AsNoTracking(), _appSettings.DefaultCacheLifeTimeInMinutes);
            });

            _logger.LogDebug("{0}   Модули загружены из БД.", "GetModulesWithCache");
            return modules;
        }

        private bool IsModuleCacheable(int moduleId)
        {
            var module = GetModulesWithCache().FirstOrDefault(t => t.Id == moduleId);
            if (module?.ModuleCacheLifeTimeInMinutes != null && module.ModuleCacheLifeTimeInMinutes == 0)
                return false;

            return true;
        }

    }
}
