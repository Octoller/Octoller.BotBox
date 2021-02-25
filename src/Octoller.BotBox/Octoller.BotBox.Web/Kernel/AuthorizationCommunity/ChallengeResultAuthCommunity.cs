using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;

namespace Octoller.BotBox.Web.Kernel.AuthorizationCommunity
{
    /// <summary>
    /// <see cref="IActionResult"/> при выполнении вызывающий <see cref="AuthorizationCommunity.HandlerAuthCommunity.HandleChalengeAsync"/>
    /// </summary>
    public class ChallengeResultAuthCommunity : IActionResult 
    {
        private readonly PropertiesAuthCommunity properties;

        /// <summary>
        /// Создает новый 
        /// </summary>
        /// <param name="properties"></param>
        public ChallengeResultAuthCommunity(PropertiesAuthCommunity properties) 
        {
            this.properties = properties;
        }

        ///<inheritdoc />
        public async Task ExecuteResultAsync(ActionContext context) 
        {
            if (context is null) 
            {
                throw new ArgumentNullException(nameof(context));
            }

            await context.HttpContext.RequestServices
                .GetRequiredService<HandlerAuthCommunity>()
                .HandleChalengeAsync(properties);
        }
    }
}
