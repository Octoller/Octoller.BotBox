using Microsoft.Extensions.DependencyInjection;
using Octoller.BotBox.Web.Kernel.Processor;

namespace Octoller.BotBox.Web.Kernel.Extension {

    public static class ServiceExtension {

        public static IServiceCollection AddVkProviderProcessor(this IServiceCollection services) {

            return services.AddScoped<VkProviderProcessor, VkProviderProcessor>();
        }
    }
}
