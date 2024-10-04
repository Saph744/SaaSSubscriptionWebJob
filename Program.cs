using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SaaSFulfillmentApp.Data;
using SaaSFulfillmentApp.Repository.Interface;
using SaaSFulfillmentApp.Services;
using SaaSFulfillmentApp.Services.Interface;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SaaSFulfillmentApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var host = new HostBuilder()
                .ConfigureWebJobs(webJobsBuilder =>
                {
                    webJobsBuilder.AddAzureStorageCoreServices();
                    webJobsBuilder.AddTimers();
                })
                .ConfigureLogging((context, b) =>
                {
                    b.SetMinimumLevel(LogLevel.Information);
                    b.AddConsole();
                })
                .ConfigureServices(services =>
                {
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
                    services.AddSingleton<IConfiguration>(configuration);
                    services.AddScoped<ISubscriptiondbRepository, SubscriptiondbRepository>();
                    services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
                    services.AddScoped<ISubscriptiondbService, SubscriptiondbService>();
                    services.AddScoped<ISubscriptionService, SubscriptionService>();
                    services.AddScoped<SubscriptionSyncService>();
                    services.AddHttpClient<TokenService>();
                    services.AddTransient<IEmailService, EmailService>();
                    services.AddHttpClient<ISubscriptionRepository, SubscriptionRepository>(client =>
                    {
                        client.BaseAddress = new Uri("https://marketplaceapi.microsoft.com/api/saas/");
                    });
                })
                .UseConsoleLifetime()
                .Build();

            using (host)
            {
                await host.RunAsync();
            }
        }
    }
}
