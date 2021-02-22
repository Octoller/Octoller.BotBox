using Microsoft.Extensions.Logging;
using System;

namespace Octoller.BotBox.Web.Logging 
{
    public class LogWriter 
    {
        /// <summary>
        /// Записывает в логи информацию о событи подключения к внутренним и\или внешним сервисам.
        /// Если передается объект Exeption подключение считается неудачным.
        /// </summary>
        /// <param name="logger">Logger текущей категории</param>
        /// <param name="services">Наименование сервиса к которому производится подключение</param>
        /// <param name="parameter">Произвольный набор параметров подключения</param>
        /// <param name="exception">Ошибка подключения.</param>
        public static void Conected(ILogger logger, string services, string parameter, Exception exception = null) 
        {
            if (exception is null)
            {
                ConectedExecuteSuccess(logger, services, parameter, null);
            }
            else
            {
                ConectedExecuteFailed(logger, services, parameter, exception);
            }
        }

        private static readonly Action<ILogger, string, string, Exception> ConectedExecuteSuccess =
            LoggerMessage.Define<string, string>(
                LogLevel.Information,
                LogsHelper.ConectedSuccess,
                "Успешное подключение к {services} c параметрами {parameter}.");

        private static readonly Action<ILogger, string, string, Exception> ConectedExecuteFailed =
            LoggerMessage.Define<string, string>(
                LogLevel.Error,
                LogsHelper.ConectedFailed,
                "Ошибка подключения к к {services} c параметрами {parameter}.");

    }
}
