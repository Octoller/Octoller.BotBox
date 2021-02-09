using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Octoller.BotBox.Web.Kernel.Extension {

    public static class VkAuthentication {

        private static readonly string displayName = "VKontakte";
        private static readonly string callbackPath = "/signin-vkontakte-token";
        private static readonly string autchEndpoints = "https://oauth.vk.com/authorize";
        private static readonly string tokenEndpoints = "https://oauth.vk.com/access_token";

        /// <summary>
        /// Добовляет в зависимости аутентификацию через сторонний сервис соцсети VKontakte
        /// </summary>
        /// <param name="builder">AuthenticationBuilder</param>
        /// <param name="configuration">Объект конфигурационных данных</param>
        /// <returns>AuthenticationBuilder</returns>
        public static AuthenticationBuilder AddVkAuthentication(this AuthenticationBuilder builder, IConfiguration configuration) =>
            
            builder.AddOAuth(AppData.ExternalAuthProvider.VkProviderName, displayName, options => {

                options.ClaimsIssuer = displayName;
                options.ClientId = configuration["Authentication:VK:AppId"];
                options.ClientSecret = configuration["Authentication:VK:AppSecret"];
                options.CallbackPath = new Microsoft.AspNetCore.Http.PathString(callbackPath);
                options.AuthorizationEndpoint = autchEndpoints;
                options.TokenEndpoint = tokenEndpoints;
                options.Scope.Add("email");
                options.Scope.Add("offline");
                options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "user_id");
                options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                options.SaveTokens = true;
                options.Events = new OAuthEvents {
                    OnCreatingTicket = context => {
                        context.RunClaimActions(context.TokenResponse.Response.RootElement);
                        return Task.CompletedTask;
                    }
                };
            });
        }
}
