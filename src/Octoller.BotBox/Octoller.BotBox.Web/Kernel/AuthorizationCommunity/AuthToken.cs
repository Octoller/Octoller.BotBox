namespace Octoller.BotBox.Web.Kernel.AuthorizationCommunity
{
    public class AuthToken 
    {
        /// <summary>
        /// Идентификатор сообщества на стороне ВК
        /// </summary>
        public string Community { get; set; }

        /// <summary>
        /// Ключ доступа
        /// </summary>
        public string Value { get; set; }
    }
}
