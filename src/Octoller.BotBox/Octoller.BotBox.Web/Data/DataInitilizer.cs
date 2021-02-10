using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Octoller.BotBox.Web.Kernel;
using Octoller.BotBox.Web.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Octoller.BotBox.Web.Data {

    public static class DataInitilizer {

        public static async Task InitializeAsync(IServiceProvider service) {

            IServiceScope scope = service.CreateScope();

            using ApplicationDbContext context = scope.ServiceProvider.GetService<ApplicationDbContext>();

            //проверяем, создана ли база данных, если нет - создаем
            bool isExists = context.GetService<IDatabaseCreator>() is RelationalDatabaseCreator databaseCreator
                && await databaseCreator.ExistsAsync();

            if (isExists) {
                return;
            }

            await context.Database.MigrateAsync();

            //создаем роли
            RoleStore<IdentityRole> roleStore = new RoleStore<IdentityRole>(context);

            foreach (string role in AppData.RolesData.Roles) {

                if (!context.Roles.Any(r => r.Name == role)) {

                    IdentityResult createRoleResult = await roleStore.CreateAsync(new IdentityRole(role) {
                        NormalizedName = role.ToUpper()
                    });

                    if (!createRoleResult.Succeeded) {
                        string message = string.Join(" | ",
                            createRoleResult.Errors.Select(x => $"{x.Code}: {x.Description}"));
                        throw new Exception(message);
                    }
                }
            }

            //создаем учетную запись админимстратора
            IConfiguration configure = context.GetService<IConfiguration>();

            string email = configure["Data:AdminData:Email"];

            User user = new User {
                Email = email,
                EmailConfirmed = true,
                NormalizedEmail = email.ToUpper(),
                UserName = email,
                NormalizedUserName = email.ToUpper()
            };

            PasswordHasher<User> passwordHasher = new PasswordHasher<User>();

            user.PasswordHash = passwordHasher.HashPassword(user, configure["Data:AdminData:Password"]);

            UserStore<User> userStroe = new UserStore<User>(context);

            IdentityResult resultCreateUser = await userStroe.CreateAsync(user);

            if (!resultCreateUser.Succeeded) {
                string message = string.Join(" | ",
                    resultCreateUser.Errors.Select(x => $"{x.Code}: {x.Description}"));
                throw new Exception(message);
            }

            // добавляем администратору роль администратора
            UserManager<User> userManager = scope.ServiceProvider.GetService<UserManager<User>>();

            foreach (string role in AppData.RolesData.Roles) {

                IdentityResult resultAddRole = await userManager.AddToRoleAsync(user, role);

                if (!resultAddRole.Succeeded) {
                    string message = string.Join(" | ",
                        resultCreateUser.Errors.Select(x => $"{x.Code}: {x.Description}"));
                    throw new Exception(message);
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
