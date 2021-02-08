using Microsoft.Extensions.Logging;
using System;

namespace Octoller.BotBox.Web.Logging {

    public class LogWriter {

        public static void Conected(ILogger logger, string vkId, Exception exception = null) {

            ConectedExecute(logger, vkId, exception);

        }

        private static readonly Action<ILogger, string, Exception> ConectedExecute =
            LoggerMessage.Define<string>(
                LogLevel.Information,
                LogsHelper.ConectedVkId,
                "Успешное подключение к сервисам VK. Внутренний Id: {vkId}");

    }
}
