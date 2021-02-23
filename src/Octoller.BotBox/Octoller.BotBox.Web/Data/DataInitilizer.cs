using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Octoller.BotBox.Web.Kernel;
using Octoller.BotBox.Web.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System;

namespace Octoller.BotBox.Web.Data
{
    /// <summary>
    /// Класс инициализации базы данных начальными значениями
    /// </summary>
    public static class DataInitilizer 
    {
        /// <summary>
        /// Инициализирует базу данныха начальными значениями
        /// </summary>
        /// <param name="service">Провайдер сервисов <see cref="IServiceProvider"/></param>
        public static async Task InitializeAsync(IServiceProvider service) 
        {
            IServiceScope scope = service.CreateScope();
            
            using ApplicationDbContext context = scope.ServiceProvider.GetService<ApplicationDbContext>();

            //проверяем, создана ли база данных, если нет - создаем
            bool isExists = context.GetService<IDatabaseCreator>() is RelationalDatabaseCreator databaseCreator
                && await databaseCreator.ExistsAsync();

            StringBuilder errorMessage = new StringBuilder();

            RoleStore<IdentityRole> roleStore = new RoleStore<IdentityRole>(context);

            foreach (string role in AppData.RolesData.Roles) 
            {
                if (!context.Roles.Any(r => r.Name == role)) 
                {
                    try
                    {
                        IdentityResult createRoleResult = await roleStore.CreateAsync(new IdentityRole(role)
                        {
                            NormalizedName = role.ToUpper()
                        });
                    }
                    catch(Exception ex)
                    {
                        errorMessage.Append("Ошибка создания ролей: ");
                        errorMessage.Append(ex.Message);
                    }
                }
            }

            //создаем учетную запись админимстратора
            IConfiguration configure = context.GetService<IConfiguration>();

            string email = configure["Data:AdminData:Email"];

            User user = new User 
            {
                Email = email,
                EmailConfirmed = true,
                NormalizedEmail = email.ToUpper(),
                UserName = email,
                NormalizedUserName = email.ToUpper()
            };

            PasswordHasher<User> passwordHasher = new PasswordHasher<User>();

            user.PasswordHash = passwordHasher.HashPassword(user, configure["Data:AdminData:Password"]);

            UserStore<User> userStroe = new UserStore<User>(context);

            try
            {
                IdentityResult resultCreateUser = await userStroe.CreateAsync(user);
            }
            catch (Exception ex)
            {
                errorMessage.Append("\nОшибка создания пользователя: ");
                errorMessage.Append(ex.Message);
            }

            // добавляем администратору роль администратора
            UserManager<User> userManager = scope.ServiceProvider.GetService<UserManager<User>>();

            foreach (string role in AppData.RolesData.Roles) 
            {
                IdentityResult resultAddRole = await userManager.AddToRoleAsync(user, role);

                if (!resultAddRole.Succeeded) 
                {
                    string message = string.Join(" | ",
                        resultAddRole.Errors.Select(x => $"{x.Code}: {x.Description}"));

                    errorMessage.Append("\nОшибка добавления роли пользователю: ");
                    errorMessage.Append(message);
                }
            }

            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                errorMessage.Append("\nОшибка сохранения данных: ");
                errorMessage.Append(ex.Message);
            }

            if (errorMessage is not null)
            {
                scope.ServiceProvider.GetService<ILogger<Program>>()?.LogError(errorMessage.ToString());
            }
        }
    }
}
