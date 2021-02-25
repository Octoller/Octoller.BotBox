using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Octoller.BotBox.Web.ViewModels;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Security.Claims;
using Octoller.BotBox.Web.Kernel.Services;
using Octoller.BotBox.Web.Kernel;
using Octoller.BotBox.Web.Data.Models;
using AutoMapper;

namespace Octoller.BotBox.Web.Controllers
{
    public class AccountController : Controller 
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly VkProviderProcessor vkProvider;
        private readonly ILogger<AccountController> logger;
        private readonly IMapper mapper;

        public AccountController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            VkProviderProcessor vkProvider,
            ILogger<AccountController> logger,
            IMapper mapper) 
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.vkProvider = vkProvider;
            this.logger = logger;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl) 
        {
            if (User.Identity.IsAuthenticated) 
            {
                return Redirect(returnUrl);
            }

            //получаем имена всех подключенных провайдеров авторизации
            var providers = await this.signInManager.GetExternalAuthenticationSchemesAsync();

            return View(new LoginViewModel 
            {
                ReturnUrl = returnUrl ?? Url.Action("Index", "Home"),
                Providers = providers
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginData) 
        {
            if (User.Identity.IsAuthenticated) 
            {
                return Redirect(loginData.ReturnUrl);
            }

            if (ModelState.IsValid) 
            {
                var user = await this.userManager.FindByEmailAsync(loginData.Email);

                if (user != null) 
                {
                    var signInResult = await this.signInManager
                        .PasswordSignInAsync(user, loginData.Password, loginData.IsPersistent, false);

                    if (signInResult.Succeeded) 
                    {
                        return Redirect(loginData.ReturnUrl);
                    }
                } 
                else 
                {
                    ModelState.AddModelError(string.Empty, "User not found");
                }
            } 

            ModelState.AddModelError("", "Неверно указан пароль или логин");

            loginData.Password = string.Empty;

            return View(loginData);
        }

        [HttpGet]
        public async Task<IActionResult> Register(string returnUrl) 
        {
            if (User.Identity.IsAuthenticated) 
            {
                return Redirect(returnUrl);
            }

            //получаем имена всех подключенных провайдеров авторизации
            var providers = await this.signInManager
                .GetExternalAuthenticationSchemesAsync();

            return View(new RegisterViewModel 
            {
                ReturnUrl = returnUrl ?? Url.Action("Index", "Home"),
                Providers = providers
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registerData) 
        {
            if (User.Identity.IsAuthenticated) 
            {
                return Redirect(registerData.ReturnUrl);
            }

            if (ModelState.IsValid) 
            {
                var user = new User 
                {
                    UserName = registerData.Email,
                    Email = registerData.Email
                };

                // создаем пользователя
                var resultCreate = await this.userManager.CreateAsync(user, registerData.Password);

                if (resultCreate.Succeeded) 
                {
                    // устанавливаем куки аутентификации
                    await signInManager.SignInAsync(user, false);

                    return Redirect(registerData.ReturnUrl);
                } 
                else 
                {
                    foreach (var error in resultCreate.Errors) 
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(registerData);
        }

        [HttpGet]
        public IActionResult ExternalLogin(string returnUrl, string providerName) 
        {
            returnUrl ??= Url.Action("Index", "Home");

            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new 
            {
                returnUrl
            });

            var properties = this.signInManager
                .ConfigureExternalAuthenticationProperties(providerName, redirectUrl);

            return Challenge(properties, providerName);        
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl) 
        {
            var loginInfo = await this.signInManager.GetExternalLoginInfoAsync();

            if (loginInfo is null) 
            {
                return RedirectToAction("Login");
            }

            var result = await this.signInManager.ExternalLoginSignInAsync(
                loginProvider: loginInfo.LoginProvider,
                providerKey: loginInfo.ProviderKey,
                isPersistent: true,
                bypassTwoFactor: false);

            if (result.Succeeded) 
            {
                return Redirect(returnUrl);
            } 
            else 
            {
                var userEmail = loginInfo.Principal.Claims
                        .Where(c => c.Type == ClaimTypes.Email)
                        .FirstOrDefault()
                        .Value;

                var user = await this.userManager.FindByEmailAsync(userEmail);

                if (user is null) 
                {
                    _ = await this.userManager.CreateAsync(new User 
                    {
                        Email = userEmail,
                        UserName = userEmail
                    });

                    user = await this.userManager.FindByEmailAsync(userEmail);
                }

                var createAccountResult = await this.vkProvider
                    .CreateVkAccountAsync(user.Id, userEmail, loginInfo);

                if (createAccountResult.Succeeded) 
                {
                    var addRoleResult = await this.userManager
                        .AddToRoleAsync(user, AppData.RolesData.UserRoleName);

                    if (addRoleResult.Succeeded) 
                    {
                        var addLoginInfoResult = await this.userManager
                            .AddLoginAsync(user, loginInfo);

                        if (addLoginInfoResult.Succeeded) 
                        {
                            await this.signInManager.SignOutAsync();
                            await this.signInManager.SignInAsync(user, true);

                            return Redirect(returnUrl);
                        }
                    }
                }
            }

            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> LogOut() 
        {
            await this.signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
