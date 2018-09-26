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
                .Build();
            
            var logFilePath = configuration.GetValue<string>("LogFilePath");
            var telegramBotToken = configuration.GetValue<string>("TelegramBotToken");
            var telegramChatId = configuration.GetValue<int>("TelegramChatId");

			var telegramLogNotifier = new TelegramLogNotifier(logFilePath, telegramBotToken, telegramChatId);
			telegramLogNotifier.Run();
        }
    }
}
