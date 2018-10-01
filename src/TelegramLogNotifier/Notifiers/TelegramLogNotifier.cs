using System;
using Microsoft.Extensions.Options;
using TelegramLogNotifier.DirectoryFileWatch;
using TelegramLogNotifier.Models;
using TelegramLogNotifier.Telegram;

namespace TelegramLogNotifier.Notifiers
{
    public class TelegramLogNotifier : IFileEventNotifier
    {
        readonly TelegramBotMessageSender _telegramBotMessageSender;
        readonly LogMessageParser _logMessageParser;

        public TelegramLogNotifier(TelegramBotMessageSender telegramBotMessageSender, LogMessageParser logMessageParser)
        {
            _telegramBotMessageSender = telegramBotMessageSender;
            _logMessageParser = logMessageParser;
        }

        public void Notify(FileEvent fileEvent)
        {
            var message = string.Empty;

            switch (fileEvent.Type)
            {
                case FileEventType.Modified:
                    message = _logMessageParser.Parse(fileEvent.Message);
                    break;
                case FileEventType.FileSizeExceeded:
                    message = $"File size exceeded for log file: {fileEvent.Message}";
                    break;
                default: throw new Exception("FileEventType not found!");
            }

            _telegramBotMessageSender.SendMessage(message);
        }

        public void Dispose()
        {
            if (_telegramBotMessageSender != null)
                _telegramBotMessageSender.Dispose();

            if (_logMessageParser != null)
                _logMessageParser.Dispose();
        }
    }
}
