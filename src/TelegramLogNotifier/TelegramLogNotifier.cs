using System;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using TelegramLogNotifier.DirectoryFileWatch;
using TelegramLogNotifier.DirectoryFileWatch.Models;

namespace TelegramLogNotifier
{
    public class TelegramLogNotifier : IDisposable
    {
        readonly DirectoryWatcher _directoryWatcher;
        readonly TelegramBotMessageSender _telegramBotMessageSender;
        readonly LogMessageParser _logMessageParser;

        public TelegramLogNotifier(string logFilesDirectory, string telegramBotToken, int telegramChatId)
        {
            _directoryWatcher = new DirectoryWatcher(logFilesDirectory, "*.log", ProcessFileChange);
            _telegramBotMessageSender = new TelegramBotMessageSender(telegramBotToken, telegramChatId);
            _logMessageParser = new LogMessageParser();
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
