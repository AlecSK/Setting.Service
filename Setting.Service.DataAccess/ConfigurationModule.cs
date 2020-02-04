using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Setting.Service.Common;

// ReSharper disable UnusedMember.Global

namespace Setting.Service.DataAccess
{

    [DebuggerDisplay("{Id} : {Name}, ModuleCacheLifeTimeInMinutes = {ModuleCacheLifeTimeInMinutes}")]
    [Table("Module", Schema="Configuration")]
    public class ConfigurationModule : ICacheable
    {

        public int Id { get; set; }

        ///<summary>
        /// Название модуля.
        ///</summary>
        public string Name { get; set; }

        ///<summary>
        /// Системное название (название системной службы, сайта), которое может использоваться в командах перезапуска.
        ///</summary>
        public string SystemName { get; set; }

        ///<summary>
        /// Признак необходимости перезагрузка после изменения настроек модуля (1 - требуется).
        ///</summary>
        public bool IsRestartRequiredAfterChangesSettings { get; set; }

        ///<summary>
        /// Идентификатор типа модуля.
        ///</summary>
        public byte ModuleTypeId { get; set; }

        ///<summary>
        /// Идентификатор группы модулей.
        ///</summary>
        public byte ModuleGroupId { get; set; }

        ///<summary>
        /// Полный путь к корневому каталогу с конфигурационными файлами модуля.
        ///</summary>
        public string ConfigurationRootPath { get; set; }

        ///<summary>
        /// Описание модуля.
        ///</summary>
        public string Description { get; set; }

        /// <summary>
        /// Срок хранения данных кеша в минутах.
        /// </summary>
        /// <remarks>
        /// Значения:
        /// NULL - использовать общую настройку сервиса SettingService.DefaultCacheLifeTimeInMinutes,
        /// 0 - кеширование настроек модуля отключено,
        /// больше нуля - количество минут, по истечении которых перечитывать данные из БД MasterData в кеш сервиса. 
        /// 
        /// Значение игнорируется для настроек модуля из таблицы Setting, у которых значения колонки Setting.SettingCacheLifeTimeInMinutes отличаются от NULL.
        /// </remarks>
        public int? ModuleCacheLifeTimeInMinutes { get; set; }

        ///<summary>
        /// Дата и время обновления записи.
        ///</summary>
        public DateTime UpdatedAt { get; set; }

        [NotMapped]
        public DateTime? LoadedInCacheAt { get; set; }

        public virtual List<ConfigurationSetting> Settings { get; }
    }
}
