namespace Octoller.BotBox.Web.Kernel.AuthorizationCommunity
{
    /// <summary>
    /// Содержит информацию, используемую для обмена кодом.
    /// </summary>
    public class CodeExchangeContext 
    {
        /// <summary>
        /// Свойство аутентификации
        /// </summary>
        public PropertiesAuthCommunity Properties { get; private set; }

        /// <summary>
        /// Код, возвращенный конечной точкой авторизации.
        /// </summary>
        public string Code { get; private set; }

        /// <summary>
        /// The redirect uri used in the authorization request.
        /// </summary>
        public string RedirectUri { get; private set; }

        /// <summary>
        /// Создает новый объект <see cref="PropertiesAuthCommunity"/>.
        /// </summary>
        /// <param name="properties"><see cref="PropertiesAuthCommunity"/>.</param>
        /// <param name="code">Код, возвращенный конечной точкой авторизации.</param>
        /// <param name="redirectUri">The redirect uri used in the authorization request.</param>
        public CodeExchangeContext(PropertiesAuthCommunity properties,
            string code, string redirectUri) 
        {
            this.Code = code;
            this.RedirectUri = redirectUri;
            this.Properties = properties;
        }
    }
}
