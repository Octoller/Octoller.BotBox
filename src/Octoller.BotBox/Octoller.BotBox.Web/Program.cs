using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Octoller.BotBox.Web {

    public class Program {

        public static void Main(string[] args) {
            IHost host = CreateHostBuilder(args).Build();

            using (IServiceScope scope = host.Services.CreateScope()) {
            
                ///TODO: ����� ������ ������������� ���� ������ ���������� �������
                
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {

                    webBuilder.UseKestrel()
                        .UseContentRoot(System.IO.Directory.GetCurrentDirectory())
                        .ConfigureAppConfiguration((context, builder) => {

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

                            if (args is not null)
                                builder.AddCommandLine(args);

                        })
                        .UseIIS()
                        .UseDefaultServiceProvider((context, options) => {
                            options.ValidateScopes = false;
                        })
                        .UseStartup<Startup>();
                });
    }
}