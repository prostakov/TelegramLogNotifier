using System;
namespace TelegramLogNotifier
{
    public class TelegramLogNotifier
    {
		readonly string _logFilePath;
		readonly TelegramBotMessageSender _telegramBotMessageSender;

		public TelegramLogNotifier(string filePath, string telegramBotToken, int telegramChatId)
        {
			_logFilePath = filePath;
            _telegramBotMessageSender = new TelegramBotMessageSender(telegramBotToken, telegramChatId);
        }

		public void Run()
		{
			using (var fileChangeNotifier = new FileChangeNotifier(_logFilePath, ProcessLine))
            {
                Console.WriteLine($"Watching changes to file: {_logFilePath}.");
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
		}

		void ProcessLine(string line)
        {
            _telegramBotMessageSender.SendMessage(line);
        }
    }
}
