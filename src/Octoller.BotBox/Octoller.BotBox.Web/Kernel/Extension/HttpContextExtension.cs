using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Octoller.BotBox.Web.Kernel.AuthorizationCommunity;
using Octoller.BotBox.Web.Kernel.Services;
using System.Threading.Tasks;

namespace Octoller.BotBox.Web.Kernel.Extension
{
    public static class HttpContextExtension
    {
        public static async Task CommunityAuthorization(this HttpContext context, TicketAuthCommunity tiket)
        {
           await context.RequestServices.GetRequiredService<VkProviderProcessor>().CommunityAuthenticateAsync(tiket);
        }
    }
}
