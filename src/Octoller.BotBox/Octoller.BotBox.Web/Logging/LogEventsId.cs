namespace Octoller.BotBox.Web.Logging {

    /// <summary>
    /// Предоставляет список идентификаторов для событий Loggera.
    /// Успешные операции имеют номер от 5000 и выше
    /// Неудачне операции имеют номер от 4000 и выше
    /// </summary>
    public static class LogEventsId {

        /// <summary>
        /// Соединение с сервисом успешно
        /// </summary>
        public const int ConectedSuccess = 5000;

        /// <summary>
        /// Создание объекта успешно
        /// </summary>
        public const int CreateSuccess = 5001;

        /// <summary>
        /// Получение объекта успешно
        /// </summary>
        public const int GetSuccess       = 5002;

        /// <summary>
        /// Обновление успешно
        /// </summary>
        public const int UpdateSuccess    = 5003;

        /// <summary>
        /// Удаление объекта успешно
        /// </summary>
        public const int DeleteSuccess    = 5004;

        /// <summary>
        /// Подключение к сервису неудачно 
        /// </summary>
        public const int ConectedFailed = 4000;

        /// <summary>
        /// Создание объекта неудачно
        /// </summary>
        public const int CreateFailed = 4001;

        /// <summary>
        /// Получение объекта неуспешно
        /// </summary>
        public const int GetFailed       = 4002;

        /// <summary>
        /// Обновление не успешно
        /// </summary>
        public const int UpdateFailed    = 4003;

        /// <summary>
        /// Удаление не успешно
        /// </summary>
        public const int DeleteFailed    = 4004;

    }
}
