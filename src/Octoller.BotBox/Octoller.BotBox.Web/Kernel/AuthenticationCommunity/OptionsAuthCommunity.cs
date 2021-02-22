using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System;
using System.Net.Http;

namespace Octoller.BotBox.Web.Kernel.AuthenticationCommunity
{
    /// <summary>
    /// Параметры конфигурации для запроса доступа к сообществам на стороне внешнего провайдера (Вконтакте)
    /// </summary>
    public class OptionsAuthCommunity 
    {
        /// <summary>
        /// Получает или задает Id приложения 
        /// </summary>
        public string ClientId { get; set; } = default;

        /// <summary>
        /// Получает или задает секретный ключ
        /// </summary>
        public string ClientSecret { get; set; } = default;

        /// <summary>
        /// Путь запроса, по которому будет возвращён ответ от внешнего поставщика.
        /// Middleware обработает данный запрос.
        /// </summary>
        public PathString CallbackPath { get; set; }

        /// <summary>
        /// Получает или задает URL, по которому будет направлен запрос прав доступа
        /// </summary>
        public string AuthorizationEndpoint { get; set; } = default;

        /// <summary>
        /// Получает или задает URL, по которому будет направлен запрос на получение ключа доступа
        /// </summary>
        public string TokenEndpoint { get; set; } = default;

        /// <summary>
        /// Версия Api стороннего провайдера
        /// </summary>
        public string ApiVersion { get; set; } = default;

        /// <summary>
        /// Получает список запрашиваемых разрешений
        /// </summary>
        public ICollection<string> Scope { get; } = new HashSet<string>();

        /// <summary>
        /// Используется для связи с сервисами ВК
        /// </summary>
        public HttpClient Backchannel { get; set; }

        /// <summary>
        /// Возвращает или задает тип, используемый для защиты данных
        /// </summary>
        public DataProtectorAuthComunnity StateDataFormat { get; set; }

        /// <summary>
        /// Проверяет, что параметры действительны и заданы. 
        /// Если это не так, вызывает исключение.
        /// </summary>
        public void Validate() 
        {
            if (string.IsNullOrEmpty(ClientId)) 
            {
                throw new ArgumentException("Parameter not specified or equal to null.", nameof(ClientId));
            }

            if (string.IsNullOrEmpty(ClientSecret)) 
            {
                throw new ArgumentException("Parameter not specified or equal to null.", nameof(ClientSecret));
            }

            if (string.IsNullOrEmpty(AuthorizationEndpoint)) 
            {
                throw new ArgumentException("Parameter not specified or equal to null.", nameof(AuthorizationEndpoint));
            }

            if (string.IsNullOrEmpty(TokenEndpoint)) 
            {
                throw new ArgumentException("Parameter not specified or equal to null.", nameof(TokenEndpoint));
            }

            if (string.IsNullOrEmpty(ApiVersion)) 
            {
                throw new ArgumentException("Parameter not specified or equal to null.", nameof(ApiVersion));
            }

            if (!CallbackPath.HasValue) 
            {
                throw new ArgumentException("Parameter not specified or equal to null.", nameof(CallbackPath));
            }
        }
    }
}
