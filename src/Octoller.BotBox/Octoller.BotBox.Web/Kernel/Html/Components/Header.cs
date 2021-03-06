﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Octoller.BotBox.Web.Data.Models;
using Octoller.BotBox.Web.Kernel.Services;
using Octoller.BotBox.Web.ViewModels;

namespace Octoller.BotBox.Web.Kernel.Html.Components
{

    public class Header : ViewComponent {

        private readonly UserManager<User> userManager;
        private readonly VkProviderProcessor vkProvider;

        public Header(UserManager<User> userManager,
            VkProviderProcessor vkProvider) {

            this.userManager = userManager;
            this.vkProvider = vkProvider;
        }

        public IViewComponentResult Invoke() {

            HeaderInfo headerInfo = new HeaderInfo {
                IsAuthenticated = User.Identity.IsAuthenticated
            };

            if (headerInfo.IsAuthenticated) {

                string id = this.userManager.FindByNameAsync(User.Identity.Name).Result?.Id;

                headerInfo.ShowName = vkProvider
                    .FindAccounByUserIdAsync(id).Result?.Name
                    ?? User.Identity.Name;
            }

            return View(headerInfo);
        }
    }
}
