using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Setting.Service.Contract.DtoModels
{
    /// <summary>Настройка</summary>
    [DebuggerDisplay("{Name} : {Value}")]
    public class DtoConfigurationSetting
    {
        /// <summary>Название</summary>
        [JsonProperty("name", Required = Required.Always)]
        [Required(AllowEmptyStrings = true)]
        public string Name { get; set; }
    
        /// <summary>Значение</summary>
        [JsonProperty("value", Required = Required.Always)]
        [Required(AllowEmptyStrings = true)]
        public string Value { get; set; }

    }
}
