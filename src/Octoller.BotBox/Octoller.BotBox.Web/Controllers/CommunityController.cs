using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Octoller.BotBox.Web.Kernel.Processor;
using Octoller.BotBox.Web.Models;
using Octoller.BotBox.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Octoller.BotBox.Web.Controllers {

    public class CommunityController : Controller {

        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly VkProviderProcessor vkProvider;
        private readonly ILogger<ProfileController> logger;

        public CommunityController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            VkProviderProcessor vkProvider,
            ILogger<ProfileController> logger) {

            this.userManager = userManager;
            this.signInManager = signInManager;
            this.vkProvider = vkProvider;
            this.logger = logger;
        }

        [HttpGet]
        [Route("communities")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> Index() {

            User user = await this.userManager.FindByNameAsync(User.Identity.Name);

            IEnumerable<CommunityViewModel> communities = vkProvider.GetCommunity(user.Id,
                    c => new CommunityViewModel {
                        Name = c.Name,
                        Photo = c.Photo,
                        Connected = c.Connected
                    });

            return View(communities);
        }
    }
}
