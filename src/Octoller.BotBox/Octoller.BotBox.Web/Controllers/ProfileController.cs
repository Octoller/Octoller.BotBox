using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Octoller.BotBox.Web.Data.Models;
using Octoller.BotBox.Web.Kernel.Services;
using Octoller.BotBox.Web.ViewModels;
using System.Threading.Tasks;

namespace Octoller.BotBox.Web.Controllers
{
    public class ProfileController : Controller 
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly VkProviderProcessor vkProvider;
        private readonly ILogger<ProfileController> logger;
        private readonly IMapper mapper;

        public ProfileController(UserManager<User> userManager,
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
        [Route("profile")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> Index() 
        {
            var user = await this.userManager.FindByNameAsync(User.Identity.Name);

            if (user is null)
            {
                ///TODO: лог ошибки
                RedirectToAction("Index", "Home");
            }

            var account = await this.vkProvider.FindAccounByUserIdAsync(user.Id);

            if (account is null) 
            {
                return View(new AccountViewModel 
                {
                    Name = User.Identity.Name,
                    IsAccountConnected = false
                });
            }

            var accountViewModel = mapper.Map<AccountViewModel>(account);

            return View(accountViewModel);
        }

        [HttpGet]
        [Authorize(Policy = "Users")]
        public IActionResult LinkAccount() 
        {
            string returnUrl = Url.Action("Index", "Profile");

            string redirectUrl = Url.Action("LinkAccountCallback", "Profile", new { returnUrl });

            AuthenticationProperties properties = this.signInManager
                .ConfigureExternalAuthenticationProperties("VK", redirectUrl);

            return Challenge(properties, "VK");
        }

        [HttpGet]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> LinkAccountCallback(string returnUrl) 
        {
            ExternalLoginInfo info = await this.signInManager.GetExternalLoginInfoAsync();                

            User user = await this.userManager.FindByNameAsync(User.Identity.Name);

            IdentityResult createAccountResult = await this.vkProvider
                .CreateVkAccountAsync(user.Id, user.Email, info);

            if (createAccountResult.Succeeded) 
            {
                IdentityResult addLoginResult = await this.userManager.AddLoginAsync(user, info);

                if (addLoginResult.Succeeded) 
                {
                    //var claim = new Claim(type: "AccountLink", value: "true");
                    //_ = await this.userManager.AddClaimAsync(user, claim);

                    await this.signInManager.SignOutAsync();
                    await this.signInManager.SignInAsync(user, true);
                }
            }

            return Redirect(returnUrl);
        }
    }
}
