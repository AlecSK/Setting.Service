using System.Diagnostics;
using Newtonsoft.Json;

namespace Setting.Service.Contract.DtoModels
{
    /// <summary>
    /// Модуль
    /// </summary>
    [DebuggerDisplay("{Id} : {SystemName}")]
    public class DtoConfigurationModule
    {
        /// <summary>Идентификатор модуля</summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>Системное название модуля</summary>
        [JsonProperty("systemName")]
        public string SystemName { get; set; }
    }
}
