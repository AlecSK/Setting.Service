using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Setting.Service.Application.Utils
{
    public static class ClientErrorsSetup
    {
        public static void Setup(IDictionary<int, ClientErrorData> dict)
        {
            dict[StatusCodes.Status400BadRequest].Title 
                = "Неправильно сформирован запрос: не хватает обязательных параметров, используются неподдерживаемые параметры, отсутствует подпись или возникла другая ошибка.";
            
            dict[StatusCodes.Status401Unauthorized].Title 
                = "Маркер доступа (access token) просрочен, аннулирован или не действует по другим причинам.";
            
            dict[StatusCodes.Status403Forbidden].Title 
                = "Маркер доступа должен иметь более широкие границы применения. Ответ может содержать требуемое значение атрибута “scope”";
            
            dict[StatusCodes.Status422UnprocessableEntity].Title 
                = "Ошибка входных данных";
            
            dict[StatusCodes.Status429TooManyRequests] 
                = new ClientErrorData {Title = "Слишком много запросов"};
            
            dict[StatusCodes.Status500InternalServerError].Title 
                = "Внутренняя ошибка";

        }

        public static void CheckErrors(this ILogger logger, Action action)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Ошибка выполнения!");
                throw;
            }
        }
    }
}
