using System.Threading.Tasks;
using System;

namespace Octoller.BotBox.Web.Kernel.AuthorizationCommunity
{
    public class AuthCommunityEvents 
    {
        /// <summary>
        /// Получает или задает делегат, который вызывается при вызове метода RedirectToAuthorizationEndpoint
        /// </summary>
        public Func<AuthCommunityEventContext, Task> OnRedirectToAuthorizationEndpoint { get; set; } = context => 
        {
            context.Response.Redirect(context.ChallengeUrl);
            return Task.CompletedTask;
        };

        /// <summary>
        /// Вызывается, когда Challenge вызывает перенаправление авторизации сообщества
        /// </summary>
        /// <param name="context">Объект контекста, содержащий Url вызова и свойства <see cref="AuthCommunityProperies"/></param>
        public Task RedirectToAuthorizationEndpoint(AuthCommunityEventContext context)
            => OnRedirectToAuthorizationEndpoint(context);
    }
}
