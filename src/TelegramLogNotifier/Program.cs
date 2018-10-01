using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace TelegramLogNotifier
{
    class Program
    {      
        static void Main(string[] args)
        {
			var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.local.json", optional: true)
                .Build();
            
            var logFilesDirectory = configuration.GetValue<string>("LogFilesDirectory");
            var telegramBotToken = configuration.GetValue<string>("TelegramBotToken");
            var telegramChatId = configuration.GetValue<int>("TelegramChatId");

			using (var telegramLogNotifier = new TelegramLogNotifier(logFilesDirectory, telegramBotToken, telegramChatId))
            {
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
        }
    }
}
