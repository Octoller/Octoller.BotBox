using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Octoller.BotBox.Web.Models;
using Octoller.BotBox.Web.ViewModels;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Security.Claims;

namespace Octoller.BotBox.Web.Controllers {

    public class AccountController : Controller {

        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly ILogger<AccountController> logger;

        public AccountController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<AccountController> logger) {

            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl) {

            if (User.Identity.IsAuthenticated) {
                return Redirect(returnUrl);
            }

            //получаем имена всех подключенных провайдеров авторизации
            IEnumerable<AuthenticationScheme> providers = await this.signInManager
                .GetExternalAuthenticationSchemesAsync();

            return View(new LoginModel {
                ReturnUrl = returnUrl ?? Url.Action("Index", "Home"),
                Providers = providers
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel loginData) {

            if (User.Identity.IsAuthenticated) {
                return Redirect(loginData.ReturnUrl);
            }

            if (ModelState.IsValid) {

                User user = await this.userManager.FindByEmailAsync(loginData.Email);

                if (user is not null) {

                    Microsoft.AspNetCore.Identity.SignInResult signInResult = await this.signInManager
                        .PasswordSignInAsync(user, loginData.Password, loginData.IsPersistent, false);

                    if (signInResult.Succeeded) {
                        return Redirect(loginData.ReturnUrl);
                    }

                } else {

                    ModelState.AddModelError(string.Empty, "User not found");
                }
            } 

            ModelState.AddModelError("", "Неверно указан пароль или логин");

            loginData.Password = string.Empty;

            return View(loginData);
        }

        [HttpGet]
        public async Task<IActionResult> Register(string returnUrl) {

            if (User.Identity.IsAuthenticated) {
                return Redirect(returnUrl);
            }

            //получаем имена всех подключенных провайдеров авторизации
            IEnumerable<AuthenticationScheme> providers = await this.signInManager
                .GetExternalAuthenticationSchemesAsync();

            return View(new RegisterModel {
                ReturnUrl = returnUrl ?? Url.Action("Index", "Home"),
                Providers = providers
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel registerData) {

            if (User.Identity.IsAuthenticated) {
                return Redirect(registerData.ReturnUrl);
            }

            if (ModelState.IsValid) {

                User user = new User {
                    UserName = registerData.Email,
                    Email = registerData.Email
                };

                // создаем пользователя
                IdentityResult resultCreate = await this.userManager
                    .CreateAsync(user, registerData.Password);

                if (resultCreate.Succeeded) {

                    // устанавливаем куки аутентификации
                    await signInManager.SignInAsync(user, false);

                    return Redirect(registerData.ReturnUrl);

                } else {

                    foreach (IdentityError error in resultCreate.Errors) {

                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(registerData);
        }

        [HttpGet]
        public IActionResult ExternalLogin(string returnUrl, string providerName) {

            returnUrl ??= Url.Action("Index", "Home");

            string redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { returnUrl });

            AuthenticationProperties properties = this.signInManager
                .ConfigureExternalAuthenticationProperties(providerName, redirectUrl);

            return Challenge(properties, providerName);        
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl) {

            ExternalLoginInfo loginInfo = await this.signInManager
                .GetExternalLoginInfoAsync();

            if (loginInfo is null) {
                return RedirectToAction("Login");
            }

            Microsoft.AspNetCore.Identity.SignInResult result = await this.signInManager.ExternalLoginSignInAsync(
                loginProvider: loginInfo.LoginProvider,
                providerKey: loginInfo.ProviderKey,
                isPersistent: true,
                bypassTwoFactor: false);

            if (result.Succeeded) {

                return Redirect(returnUrl);

            } else {

                string userEmail = loginInfo.Principal.Claims
                        .Where(c => c.Type == ClaimTypes.Email)
                        .FirstOrDefault()
                        .Value;

                User user = await this.userManager.FindByEmailAsync(userEmail);

                if (user is null) {

                    _ = await this.userManager.CreateAsync(new User {
                        Email = userEmail,
                        UserName = userEmail
                    });

                    user = await this.userManager.FindByEmailAsync(userEmail);
                }

                ///TODO: здесь нужно будет подключиться к API VK и создать запись в базе с полученными данными

                IdentityResult addLoginInfoResult = await this.userManager
                    .AddLoginAsync(user, loginInfo);

                if (addLoginInfoResult.Succeeded) {

                    await this.signInManager.SignOutAsync();
                    await this.signInManager.SignInAsync(user, true);

                    return Redirect(returnUrl);
                }
            }

            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> LogOut() {

            await this.signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
