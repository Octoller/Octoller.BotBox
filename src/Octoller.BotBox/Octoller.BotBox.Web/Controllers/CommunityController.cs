using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Octoller.BotBox.Web.Kernel.Services;
using Octoller.BotBox.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using Octoller.BotBox.Web.Kernel.AuthorizationCommunity;
using Octoller.BotBox.Web.Data.Models;
using AutoMapper;

namespace Octoller.BotBox.Web.Controllers
{
    public class CommunityController : Controller 
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly VkProviderProcessor vkProvider;
        private readonly ILogger<ProfileController> logger;
        private readonly IMapper mapper;

        public CommunityController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            VkProviderProcessor vkProvider,
            ILogger<ProfileController> logger,
            IMapper mapper) 
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.vkProvider = vkProvider;
            this.logger = logger;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("communities")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> Index() 
        {
            var user = await this.userManager.FindByNameAsync(User.Identity.Name);

            var communities = vkProvider.GetCommunity(user.Id);
            var communityViewModels = new List<CommunityViewModel>();
            
            foreach(var community in communities)
            {
                var c = mapper.Map<CommunityViewModel>(community);
                communityViewModels.Add(c);
            }

            return View(communityViewModels);
        }

        [HttpPost]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> AuthorizeCommunity(string id) 
        {
            if (id is null) 
            {
                return RedirectToAction("Index");
            }

            var redirectUri = Url.Action("AuthorizeCommunityCallback", "Community");

            var properties = await this.vkProvider.GetRequestUrlForAuthorizeCommunityAsync(id, redirectUri);

            if (properties.IsEmpty) 
            {
                ///TODO: запись в лог об ошибке
                return RedirectToAction("Index", "Community");
            }
            
            return AuthCommunityChallenge(properties);
        }

        //public async Task<IActionResult> UnAuthorizeCommunity(string id)
        //{
        //    if (id is null)
        //    {
        //        return RedirectToAction("Index");
        //    }
        //}

        public IActionResult AuthorizeCommunityCallback() 
        {
            return RedirectToAction("Index", "Community");
        }

        private IActionResult AuthCommunityChallenge(PropertiesAuthCommunity properties) 
            => new ChallengeResultAuthCommunity(properties);
    }
}
