using System.Collections.Generic;
using Setting.Service.Contract.DtoModels;

namespace Setting.Service.Contract.Interfaces
{
    public interface ICachingRepository
    {
        List<DtoConfigurationSetting> GetDtoSettingValuesByModuleIdWithCache(int moduleId, bool refreshCache = false);

        string Get1SettingValueByModuleId(int moduleId, string settingName);

        List<DtoConfigurationModule> GetDtoModulesWithCache();

        void ResetCache();
    }
}
