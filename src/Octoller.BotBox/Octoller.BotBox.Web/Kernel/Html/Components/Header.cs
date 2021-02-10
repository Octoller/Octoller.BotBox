using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Octoller.BotBox.Web.Models;
using Octoller.BotBox.Web.ViewModels;

namespace Octoller.BotBox.Web.Kernel.Html.Components {

    public class Header : ViewComponent {

        private readonly UserManager<User> userManager;

        public Header(UserManager<User> userManager) {
            this.userManager = userManager;
        }

        public IViewComponentResult Invoke() {

            HeaderInfo headerInfo = new HeaderInfo {
                IsAuthenticated = User.Identity.IsAuthenticated
            };

            if (headerInfo.IsAuthenticated) {

                headerInfo.ShowName = User.Identity.Name;
            }

            return View(headerInfo);
        }
    }
}
