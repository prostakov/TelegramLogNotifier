﻿using System;
using System.Text;
using Newtonsoft.Json;

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
            var log = JsonConvert.DeserializeObject<Log>(line);

            var message = GetMessage(log);

            _telegramBotMessageSender.SendMessage(message);
        }

        string GetMessage(Log log)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("<b>" + log.Level + "</b>");
            sb.AppendLine("<i>" + log.Timestamp.ToString() + "</i>");
            sb.AppendLine(log.MessageTemplate);

            return sb.ToString();
        }
    }
}