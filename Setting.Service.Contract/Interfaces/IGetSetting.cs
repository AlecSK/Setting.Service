using System.Collections.Generic;
using Setting.Service.Contract.DtoModels;

namespace Setting.Service.Contract.Interfaces
{
    interface IGetSetting
    {
        /// <summary>Получение информации о модулях системы.</summary>
        IEnumerable<DtoConfigurationModule> GetModules();
    
        /// <summary>Получение текстового значения настройки по названию и идентификатору модуля.</summary>
        /// <param name="moduleId">Идентификатор модуля</param>
        /// <param name="settingName">Название настройки</param>
        string GetSettingValueByModuleId(int moduleId, string settingName);
    
        /// <summary>Получение словаря текстовых значений настроек модуля по его идентификатору.</summary>
        /// <param name="moduleId">Идентификатор модуля</param>
        /// <param name="useCache">Признак использования кеширования. Если false - получить значения из БД MasterData с обновлением кеша сервиса.</param>
        IEnumerable<DtoConfigurationSetting> GetSettingValuesByModuleId(int moduleId, bool useCache);
    
    }
}
