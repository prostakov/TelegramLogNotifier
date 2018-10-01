using System;
using Microsoft.Extensions.Options;
using TelegramLogNotifier.DirectoryFileWatch;
using TelegramLogNotifier.interfaces;
using TelegramLogNotifier.Models;
using TelegramLogNotifier.Telegram;

namespace TelegramLogNotifier.Notifiers
{
    public class LogMessageNotifier : IFileEventNotifier
    {
        readonly IMessageSender _messageSender;
        readonly LogMessageParser _logMessageParser;

        public LogMessageNotifier(IMessageSender messageSender, LogMessageParser logMessageParser)
        {
            _messageSender = messageSender;
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

            _messageSender.Send(message);
        }

        public void Dispose()
        {
            if (_messageSender != null)
                _messageSender.Dispose();

            if (_logMessageParser != null)
                _logMessageParser.Dispose();
        }
    }
}
