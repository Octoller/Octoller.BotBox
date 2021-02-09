using Microsoft.Extensions.Logging;

namespace Octoller.BotBox.Web.Logging {

    /// <summary>
    /// Предоставляет набор объектов структуры EventId для событий логирования
    /// </summary>
    public static class LogsHelper {

        /// <summary>
        /// Указывает на успешное подключение к сервису
        /// </summary>
        public static readonly EventId ConectedSuccess = new EventId(
            id: LogEventsId.ConectedSuccess,
            name: "ConectedSuccess");

        /// <summary>
        /// Указывает на неудачное подключение к сервису
        /// </summary>
        public static readonly EventId ConectedFailed = new EventId(
            id: LogEventsId.ConectedFailed,
            name: "ConectedFailed");

    }
}
