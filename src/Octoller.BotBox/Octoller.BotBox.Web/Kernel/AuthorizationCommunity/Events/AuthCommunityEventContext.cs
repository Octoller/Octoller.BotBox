using Microsoft.AspNetCore.Http;

namespace Octoller.BotBox.Web.Kernel.AuthorizationCommunity
{
    /// <summary>
    /// Контекст данных для события перенаправления при аутентификации сообщества
    /// </summary>
    public class AuthCommunityEventContext 
    {
        public AuthCommunityEventContext(
            PropertiesAuthCommunity properties,
            OptionsAuthCommunity options,
            HttpContext context,
            string challengeUrl) 
        {
            Properties = properties;
            Options = options;
            HttpContext = context;
            ChallengeUrl = challengeUrl;
        }

        /// <summary>
        /// Предоставляет url-адрес запроса
        /// </summary>
        public string ChallengeUrl { get; private set; }

        /// <summary>
        /// Предоставляет <see cref="PropertiesAuthCommunity"/>
        /// </summary>
        public PropertiesAuthCommunity Properties { get; private set; }

        /// <summary>
        /// Предоставляет <see cref="OptionsAuthCommunity"/>
        /// </summary>
        public OptionsAuthCommunity Options { get; private set; }

        /// <summary>
        /// Предоставляет <see cref="Microsoft.AspNetCore.Http.HttpContext"/>
        /// </summary>
        public HttpContext HttpContext { get; private set; }

        /// <summary>
        /// Предоставляет Request заданного <see cref="Microsoft.AspNetCore.Http.HttpContext"/>
        /// </summary>
        public HttpRequest Request => HttpContext.Request;

        /// <summary>
        /// Предоставляет Response заданного <see cref="Microsoft.AspNetCore.Http.HttpContext"/>
        /// </summary>
        public HttpResponse Response => HttpContext.Response;
    }
}
