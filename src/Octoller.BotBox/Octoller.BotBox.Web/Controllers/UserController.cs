using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Octoller.BotBox.Web.Models;
using System.Threading.Tasks;

namespace Octoller.BotBox.Web.Controllers {

    public class UserController : Controller {

        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly ILogger<UserController> logger;

        public UserController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<UserController> logger) {

            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }


        

        [HttpGet]
        public async Task<IActionResult> LogOut() {

            await this.signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
