using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Octoller.BotBox.Web 
{
    public class Program 
    {
        public static void Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();

            using (IServiceScope scope = host.Services.CreateScope()) 
            {
                Data.DataInitilizer.InitializeAsync(scope.ServiceProvider).GetAwaiter().GetResult();
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) 
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => 
                {
                    webBuilder.ConfigureAppConfiguration((context, builder) => 
                        {
                            builder.AddJsonFile(
                                path: "appsettings.json",
                                optional: true, reloadOnChange: true);

                            builder.AddJsonFile(
                                path: $"appsettings.{context.HostingEnvironment.EnvironmentName}.json",
                                optional: true, reloadOnChange: true);

                            builder.AddJsonFile(
                                path: "privatesettings.json",
                                optional: true, reloadOnChange: true);

                            builder.AddEnvironmentVariables();

                            if (args != null)
                                builder.AddCommandLine(args);

                        })
                        .UseKestrel()
                        .UseDefaultServiceProvider((context, options) => 
                        {
                            options.ValidateScopes = false;
                        })
                        .UseStartup<Startup>();
                });
        }
    }
}
