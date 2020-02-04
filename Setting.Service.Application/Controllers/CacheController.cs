using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Setting.Service.Application.Utils;
using Setting.Service.Contract.Interfaces;

namespace Setting.Service.Application.Controllers
{

    [ApiController]
    [Authorize]
    [Route("[controller]")]
    [Produces("application/json")]
    public class CacheController : ControllerBase
    {
        private readonly ILogger<CacheController> _logger;
        private readonly ICachingRepository _cachingRepository;

        public CacheController(ILoggerFactory loggerFactory, ICachingRepository cachingRepository)
        {
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

            _logger = loggerFactory.CreateLogger<CacheController>();

            _cachingRepository = cachingRepository ?? throw new ArgumentNullException(nameof(cachingRepository));
            
            _logger.LogInformation("Контроллер инициализирован.");
        }

        /// <summary>
        /// Сброс кеша сервиса.
        /// </summary>
        /// <remarks>
        /// Пример использования:
        ///
        ///     GET /cache/reset
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
        [HttpGet("Reset", Name = "CacheReset")]
        [ApiConventionMethod(typeof(ResponseConventions), nameof(ResponseConventions.Standard))]
        public IActionResult CacheReset()
        {
            _logger.LogDebug("{0}   Вызываем очистку кэша.", "CacheReset");

            _cachingRepository.ResetCache();

            return Ok();
        }
    }
}
