using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Octoller.BotBox.Web.Data.Stores;
using Octoller.BotBox.Web.Kernel;
using Octoller.BotBox.Web.Kernel.AuthenticationCommunity;
using Octoller.BotBox.Web.Kernel.Extension;
using Octoller.BotBox.Web.Kernel.Middleware;
using Octoller.BotBox.Web.Kernel.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Octoller.BotBox.Web 
{
    public class Startup 
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration) 
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services) 
        {
            services.AddDataProtection();

            //подключаем базу данных
            services.AddDbContext<Data.ApplicationDbContext>(options => 
            {
                options.UseSqlServer(this.configuration["ConnectionStrings:DbConnection"]);
            });

            //настройка identity
            services.AddIdentity<Models.User, IdentityRole>(options => 
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<Data.ApplicationDbContext>()
            .AddDefaultTokenProviders();

            //настройка аутентификации ччерез вк
            services.AddAuthentication()
                .AddOAuth(AppData.ExternalAuthProvider.VkProviderName, "VKontakte", ConfigureOptions);

            //настройка авторизации
            services.AddAuthorization(options => 
            {
                options.AddPolicy("Users", policy => 
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole(AppData.RolesData.Roles);
                });

                options.AddPolicy("Administration", policy => 
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole(AppData.RolesData.AdministratorRoleName);
                });
            });

            services.AddTransient<VkDataStore>()
                .AddScoped<VkProviderProcessor>()
                .AddScoped<DataProtectorAuthComunnity>()
                .AddScoped<HandlerAuthCommunity>()
                .AddConnectionOptions(configuration);

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) 
        {

            if (env.IsDevelopment()) 
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages();
            } 
            else 
            {
                ///TODO: перенаправление на страницу ошибки
                ///app.UseExceptionHandler("/Home/Error");
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<CommunityConectionMiddleware>();

            app.UseEndpoints(endpoints => 
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

            });
        }

        private void ConfigureOptions(OAuthOptions options) 
        {
            options.ClaimsIssuer = "Vkontakte";
            options.ClientId = configuration["VkOptionsData:ClientId"];
            options.ClientSecret = configuration["VkOptionsData:ClientSecret"];
            options.CallbackPath = new PathString(configuration["VkOptionsData:CallbackPathAccount"]);
            options.AuthorizationEndpoint = configuration["VkOptionsData:AuthorizationEndpoint"];
            options.TokenEndpoint = configuration["VkOptionsData:TokenEndpoint"];
            options.Scope.Add("email");
            options.Scope.Add("offline");
            options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "user_id");
            options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
            options.SaveTokens = true;

            options.Events = new OAuthEvents 
            {
                OnCreatingTicket = context => 
                {
                    context.RunClaimActions(context.TokenResponse.Response.RootElement);
                    return Task.CompletedTask;
                }
            };
        }
    }
}
