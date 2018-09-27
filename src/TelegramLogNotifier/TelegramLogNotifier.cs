using System;
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
			using (var fileChangeNotifier = new FileChangeNotifier(_logFilePath, ProcessFileChange, AlertFileSizeExceeded))
            {
                Console.WriteLine($"Watching changes to file: {_logFilePath}.");
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
		}

		void ProcessFileChange(string line)
        {
            var log = JsonConvert.DeserializeObject<Log>(line);

            var message = GetMessage(log);

            _telegramBotMessageSender.SendMessage(message);
        }

        void AlertFileSizeExceeded()
        {
            var message = $"File size exceeded for log file: {_logFilePath}";

            _telegramBotMessageSender.SendMessage(message);
        }

        string GetMessage(Log log)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine($"<b>{log.Level}</b> - {log.Timestamp}");
            sb.AppendLine($"<code>{log.MessageTemplate}</code>");

            if (log.Properties.ContainsKey("SourceContext"))
            {
                sb.AppendLine($"Context: {log.Properties["SourceContext"]}");
            }

            if (log.Properties.ContainsKey("RequestPath"))
            {
                sb.AppendLine($"RequestPath: {log.Properties["RequestPath"]}");
            }

            return sb.ToString();
        }
    }
}
