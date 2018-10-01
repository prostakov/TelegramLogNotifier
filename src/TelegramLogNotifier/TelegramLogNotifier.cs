using System;
using Microsoft.Extensions.Options;
using TelegramLogNotifier.DirectoryFileWatch;
using TelegramLogNotifier.DirectoryFileWatch.Models;
using TelegramLogNotifier.Telegram;

namespace TelegramLogNotifier
{
    public class TelegramLogNotifier : IDisposable
    {
        readonly DirectoryWatcher _directoryWatcher;
        readonly TelegramBotMessageSender _telegramBotMessageSender;
        readonly LogMessageParser _logMessageParser;

        public TelegramLogNotifier(IOptions<DirectoryFileWatchSettings> directoryFileWatchConfiguration, TelegramBotMessageSender telegramBotMessageSender, LogMessageParser logMessageParser)
        {
            _directoryWatcher = new DirectoryWatcher(directoryFileWatchConfiguration.Value.LogFilesDirectory, "*.log", ProcessFileChange);
            _telegramBotMessageSender = telegramBotMessageSender;
            _logMessageParser = logMessageParser;
        }

        void ProcessFileChange(FileEvent evnt)
        {
            var message = string.Empty;

            switch (evnt.Type)
            {
                case FileEventType.Modified:
                    message = _logMessageParser.Parse(evnt.Message);
                    break;
                case FileEventType.FileSizeExceeded:
                    message = $"File size exceeded for log file: {evnt.Message}";
                    break;
                default: throw new Exception("FileEventType not found!");
            }

            _telegramBotMessageSender.SendMessage(message);
        }

        public void Dispose()
        {
            if (_directoryWatcher != null)
                _directoryWatcher.Dispose();

            if (_telegramBotMessageSender != null)
                _telegramBotMessageSender.Dispose();

            if (_logMessageParser != null)
                _logMessageParser.Dispose();
        }
    }
}
