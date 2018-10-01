using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TelegramLogNotifier.DirectoryFileWatch;
using TelegramLogNotifier.Telegram;

namespace TelegramLogNotifier
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = GetServiceProvider();

            using (var app = serviceProvider.GetService<TelegramLogNotifier>())
            {
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
        }

        static ServiceProvider GetServiceProvider()
        {
            var services = new ServiceCollection();

            services.AddSingleton(new LoggerFactory()
              .AddConsole()
              .AddDebug());
            services.AddLogging();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.local.json", optional: true)
                .Build();
            services.AddOptions();
            services.Configure<DirectoryFileWatchSettings>(configuration.GetSection("DirectoryFileWatch"));
            services.Configure<TelegramSettings>(configuration.GetSection("Telegram"));

            services.AddTransient<LogMessageParser>();
            services.AddTransient<TelegramLogNotifier>();

            return services.BuildServiceProvider();
        }
    }
}
