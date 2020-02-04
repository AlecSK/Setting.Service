using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Setting.Service.Common;

// ReSharper disable UnusedAutoPropertyAccessor.Global

// ReSharper disable UnusedMember.Global

namespace Setting.Service.DataAccess
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    [DebuggerDisplay("{Id} : {Name}, SettingCacheLifeTimeInMinutes = {SettingCacheLifeTimeInMinutes}")]
    [Table("Setting", Schema="Configuration")]
    public class ConfigurationSetting : ICacheable
    {
        public int Id { get; set; }

        ///<summary>
        /// Наименование настройки.
        ///</summary>
        public string Name { get; set; }

        ///<summary>
        /// Идентификатор типа данных: 0 - строка; 1 - целое число; 2 - дробное число, 3 - логический тип, 4 - дата и время в формате 'гггг-мм-дд чч:ми:сс'
        ///</summary>
        public byte DataTypeId { get; set; }

        ///<summary>
        /// Значение настройки в виде строки.
        ///</summary>
        public string Value { get; set; }

        ///<summary>
        /// Идентификатор модуля (null - если применимо для всех модулей).
        ///</summary>
        public int? ModuleId { get; set; }

        ///<summary>
        /// Идентификатор шаблона преобразования конфигурационного файла.
        ///</summary>
        public int? ConfigurationFileId { get; set; }

        ///<summary>
        /// Описание настройки.
        ///</summary>
        public string Description { get; set; }

        /// <summary>
        /// Срок хранения данных кеша в минутах.
        /// </summary>
        /// <remarks>
        /// Значения:
        /// NULL - использовать общую настройку модуля Module.ModuleCacheLifeTimeInMinutes или сервиса SettingService.DefaultCacheLifeTimeInMinutes (указано в порядке уменьшения приоритета),
        /// 0 - кеширование настройки отключено,
        /// больше нуля - количество минут, по истечении которых перечитывать данные из БД MasterData в кеш сервиса.
        /// </remarks>
        public int? SettingCacheLifeTimeInMinutes { get; set; }

        ///<summary>
        /// Дата и время обновления записи.
        ///</summary>
        public DateTime UpdatedAt { get; set; }


        [NotMapped]
        public DateTime? LoadedInCacheAt { get; set; }


        [ForeignKey("ModuleId")]
        public virtual ConfigurationModule ConfigurationModule { get; set; }


        public bool IsExpired(int defaultLifeTimeInMinutes)
        {
            if (!LoadedInCacheAt.HasValue)
                return true;

            int maxLifeTimeInMinutes = defaultLifeTimeInMinutes;

            if (SettingCacheLifeTimeInMinutes != null)
                if (SettingCacheLifeTimeInMinutes.Value == 0)
                    return true;
                else
                    maxLifeTimeInMinutes = SettingCacheLifeTimeInMinutes.Value;

            var diff = (DateTime.Now - LoadedInCacheAt.Value).Minutes;
            return diff > maxLifeTimeInMinutes;
        }
    }
}
