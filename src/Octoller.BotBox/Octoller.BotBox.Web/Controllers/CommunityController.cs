using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Octoller.BotBox.Web.Kernel.Services;
using Octoller.BotBox.Web.Models;
using Octoller.BotBox.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using Octoller.BotBox.Web.Kernel.AuthenticationCommunity;

namespace Octoller.BotBox.Web.Controllers 
{
    public class CommunityController : Controller 
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly VkProviderProcessor vkProvider;
        private readonly ILogger<ProfileController> logger;

        public CommunityController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            VkProviderProcessor vkProvider,
            ILogger<ProfileController> logger) 
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.vkProvider = vkProvider;
            this.logger = logger;
        }

        [HttpGet]
        [Route("communities")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> Index() 
        {
            User user = await this.userManager.FindByNameAsync(User.Identity.Name);

            IEnumerable<CommunityViewModel> communities = vkProvider.GetCommunity(user.Id,
                    c => new CommunityViewModel 
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Photo = c.Photo,
                        Connected = c.Connected
                    });

            return View(communities);
        }

        [HttpPost]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> LinkCommunity(string id) 
        {
            if (id is null) 
            {
                return RedirectToAction("Index");
            }

            string redirectUri = Url.Action("LinkCommunityCallback", "Community");

            PropertiesAuthCommunity properties = await this.vkProvider
                .GetRequestUrlForAuthorizeCommunityAsync(id, redirectUri);

            if (properties.IsEmpty) 
            {
                ///TODO: запись в лог об ошибке
                return RedirectToAction("Index", "Community");
            }
            
            return AuthCommunityChallenge(properties);
        }


        public IActionResult LinkCommunityCallback() 
        {
            return RedirectToAction("Index", "Community");
        }

        private IActionResult AuthCommunityChallenge(PropertiesAuthCommunity properties) 
            => new ChallengeResultAuthCommunity(properties);
    }
}
