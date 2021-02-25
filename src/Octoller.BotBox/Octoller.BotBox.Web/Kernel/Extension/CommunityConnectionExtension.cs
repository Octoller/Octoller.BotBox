using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Octoller.BotBox.Web.Kernel.AuthorizationCommunity;

namespace Octoller.BotBox.Web.Kernel.Extension {

    public static class CommunityConnectionExtension {

        public static IServiceCollection AddConnectionOptions(this IServiceCollection services, IConfiguration configuration) {

            services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<OptionsAuthCommunity>, PostConfigureOptionsAuthCommunity>());

            void СofigureOptions(OptionsAuthCommunity options) {
                options.ClientId = configuration["VkOptionsData:ClientId"];
                options.ClientSecret = configuration["VkOptionsData:ClientSecret"];
                options.CallbackPath = new Microsoft.AspNetCore.Http.PathString(configuration["VkOptionsData:CallbackPathCommunity"]);
                options.AuthorizationEndpoint = configuration["VkOptionsData:AuthorizationEndpoint"];
                options.TokenEndpoint = configuration["VkOptionsData:TokenEndpoint"];
                options.ApiVersion = configuration["VkOptionsData:ActualVersion"];
                options.Scope.Add("manage");
                options.Scope.Add("messages");
                options.Scope.Add("photos");
                options.Scope.Add("docs");
            }

            services.AddOptions<OptionsAuthCommunity>()
                .Configure(СofigureOptions)
                .Validate(o => {
                    o.Validate();
                    return true;
                });

            return services;

        }
    }
}
