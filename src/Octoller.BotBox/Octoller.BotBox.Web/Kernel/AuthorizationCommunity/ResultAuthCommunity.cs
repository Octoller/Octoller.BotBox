using System;

namespace Octoller.BotBox.Web.Kernel.AuthorizationCommunity
{
    /// <summary>
    /// Предоставляет результат вызова аутентификации
    /// </summary>
    public class ResultAuthCommunity 
    {
        /// <summary>
        /// Указывает, что доступ к подключению был получен
        /// и дальнейшая обработка запросса может быть пропущена
        /// </summary>
        public bool Handlered { get; private set; }

        /// <summary>
        /// Указывает, что дальнейшая логика получения доступа к подключению должны быть пропущенна
        /// и должна быть выполненна остальная часть конвеера обработки запроа
        /// </summary>
        public bool Skippeded { get; private set; }

        /// <summary>
        /// Указывает, прошло ли получение доступа успешно
        /// </summary>
        public bool Succeeded { get; private set; }

        /// <summary>
        /// Содержит информацию об ошибке получения доступа к подключению
        /// </summary>
        public Exception Failure { get; private set; }

        public TicketAuthCommunity Tiket { get; private set; }

        public PropertiesAuthCommunity Properties { get; private set; }

        public static ResultAuthCommunity Success(TicketAuthCommunity ticket)
        {
            return new ResultAuthCommunity 
            {
                Succeeded = true,
                Tiket = ticket,
                Properties = ticket.Properties
            };
        }
            

        public static ResultAuthCommunity Handled()
        {
            return new ResultAuthCommunity
            {
                Handlered = true
            };
        }

        public static ResultAuthCommunity Skipped()
        {
            return new ResultAuthCommunity
            {
                Skippeded = true
            };
        }

        public static ResultAuthCommunity Fail(Exception failure, PropertiesAuthCommunity properties) 
        {
            return new ResultAuthCommunity
            {
                Failure = failure,
                Properties = properties
            };
        }

        public static ResultAuthCommunity Fail(string failureMessage, PropertiesAuthCommunity properties)
        {
            return new ResultAuthCommunity
            {
                Failure = new Exception(failureMessage),
                Properties = properties
            };
        }
    }
}
