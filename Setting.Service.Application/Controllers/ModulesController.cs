using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Setting.Service.Application.Interfaces;
using Setting.Service.Application.Utils;
using Setting.Service.Contract.DtoModels;
using Setting.Service.Contract.Interfaces;

namespace Setting.Service.Application.Controllers
{
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    [Route("[controller]")]
    //[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    //[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status401Unauthorized)]
    //[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status500InternalServerError)]
    //[ApiConventionType(typeof(ResponseConventions))]
    public class ModulesController : ControllerBase, IReadSetting
    {
        private readonly ILogger<ModulesController> _logger;
        private readonly ICachingRepository _cachingRepository;

        public ModulesController(ILoggerFactory loggerFactory, ICachingRepository cachingRepository)
        {
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

            _logger = loggerFactory.CreateLogger<ModulesController>();

            _cachingRepository = cachingRepository ?? throw new ArgumentNullException(nameof(cachingRepository));

            _logger.LogInformation("Контроллер инициализирован.");
        }



        /// <summary>
        /// Получение информации о модулях системы.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Пример использования:
        ///
        ///     GET /modules
        /// </remarks>
        /// <response code="400">
        /// Неправильно сформирован запрос: не хватает обязательных параметров, используются неподдерживаемые параметры, отсутствует подпись или возникла другая ошибка.
        /// </response>
        /// <response code="401">
        /// Маркер доступа (access token) просрочен, аннулирован или не действует по другим причинам.
        /// </response>
        /// <response code="403">
        /// Маркер доступа должен иметь более широкие границы применения. Ответ может содержать требуемое значение атрибута “scope”
        /// </response>
        /// <response code="422">
        /// Ошибка входных данных
        /// </response>
        /// <response code="429">
        /// Слишком много запросов
        /// </response>
        /// <response code="500">
        /// Внутренняя ошибка
        /// </response>
        [HttpGet("", Name = "GetModules")]
        public ActionResult<IEnumerable<DtoConfigurationModule>> GetModules()
        {
            _logger.LogDebug("HttpGet {0}", "GetModules");
            var res = _cachingRepository.GetDtoModulesWithCache();

            if (res.Any())
                return Ok(res);

            return Ok();
        }



        /// <summary>
        /// Получение текстового значения настройки по названию и идентификатору модуля.
        /// </summary>
        /// <param name="moduleId">Идентификатор модуля</param>
        /// <param name="settingName">Название настройки</param>
        /// <returns></returns>
        /// <remarks>
        /// Пример использования:
        ///
        ///     GET /modules/127/settings/DefaultCacheLifeTimeInMinutes/value
        /// </remarks> 
        /// <response code="400">
        /// Неправильно сформирован запрос: не хватает обязательных параметров, используются неподдерживаемые параметры, отсутствует подпись или возникла другая ошибка.
        /// </response>
        /// <response code="401">
        /// Маркер доступа (access token) просрочен, аннулирован или не действует по другим причинам.
        /// </response>
        /// <response code="403">
        /// Маркер доступа должен иметь более широкие границы применения. Ответ может содержать требуемое значение атрибута “scope”
        /// </response>
        /// <response code="422">
        /// Ошибка входных данных
        /// </response>
        /// <response code="429">
        /// Слишком много запросов
        /// </response>
        /// <response code="500">
        /// Внутренняя ошибка
        /// </response>
        [HttpGet("{moduleId}/settings/{settingName}/value", Name = "GetSettingValueByModuleId")]
        [ApiConventionMethod(typeof(ResponseConventions), nameof(ResponseConventions.Standard))]
        public ActionResult<string> GetSettingValueByModuleId(int moduleId, string settingName)
        {
            _logger.LogDebug("HttpGet {0}", "GetSettingValueByModuleId");

            var res = _cachingRepository.Get1SettingValueByModuleId(moduleId, settingName);
            return Ok(res);
        }



        /// <summary>
        /// Получение словаря текстовых значений настроек модуля по его идентификатору.
        /// </summary>
        /// <param name="moduleId">Идентификатор модуля</param>
        /// <param name="useCache">Признак использования кеширования. Если false - получить значения из БД MasterData с обновлением кеша сервиса.</param>
        /// <returns></returns>
        /// <remarks>
        /// Пример использования:
        ///
        ///     GET /modules/5/true/settings/values
        /// </remarks> 
        /// <response code="400">
        /// Неправильно сформирован запрос: не хватает обязательных параметров, используются неподдерживаемые параметры, отсутствует подпись или возникла другая ошибка.
        /// </response>
        /// <response code="401">
        /// Маркер доступа (access token) просрочен, аннулирован или не действует по другим причинам.
        /// </response>
        /// <response code="403">
        /// Маркер доступа должен иметь более широкие границы применения. Ответ может содержать требуемое значение атрибута “scope”
        /// </response>
        /// <response code="422">
        /// Ошибка входных данных
        /// </response>
        /// <response code="429">
        /// Слишком много запросов
        /// </response>
        /// <response code="500">
        /// Внутренняя ошибка
        /// </response>
        [HttpGet("{moduleId}/{useCache}/settings/values", Name = "GetSettingValuesByModuleId")]
        [ApiConventionMethod(typeof(ResponseConventions), nameof(ResponseConventions.Standard))]
        public ActionResult<IEnumerable<DtoConfigurationSetting>> GetSettingValuesByModuleId(int moduleId, bool useCache)
        {
            _logger.LogDebug("HttpGet {0}", "GetSettingValuesByModuleId");

            var res = _cachingRepository.GetDtoSettingValuesByModuleIdWithCache(moduleId, !useCache);
            return Ok(res);
        }

    }
}
