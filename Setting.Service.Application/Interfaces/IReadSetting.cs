using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Setting.Service.Contract.DtoModels;

namespace Setting.Service.Application.Interfaces
{
    interface IReadSetting
    {
        /// <summary>Получение информации о модулях системы.</summary>
        ActionResult<IEnumerable<DtoConfigurationModule>> GetModules();
    
        /// <summary>Получение текстового значения настройки по названию и идентификатору модуля.</summary>
        /// <param name="moduleId">Идентификатор модуля</param>
        /// <param name="settingName">Название настройки</param>
        ActionResult<string> GetSettingValueByModuleId(int moduleId, string settingName);
    
        /// <summary>Получение словаря текстовых значений настроек модуля по его идентификатору.</summary>
        /// <param name="moduleId">Идентификатор модуля</param>
        /// <param name="useCache">Признак использования кеширования. Если false - получить значения из БД MasterData с обновлением кеша сервиса.</param>
        ActionResult<IEnumerable<DtoConfigurationSetting>> GetSettingValuesByModuleId(int moduleId, bool useCache);
    
    }
}
