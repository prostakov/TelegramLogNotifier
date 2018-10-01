﻿using System;
using Microsoft.Extensions.Options;
using TelegramLogNotifier.DirectoryFileWatch;
using TelegramLogNotifier.DirectoryFileWatch.Models;

namespace TelegramLogNotifier
{
    public class TelegramLogNotifier : IDisposable
    {
        readonly DirectoryWatcher _directoryWatcher;
        readonly TelegramBotMessageSender _telegramBotMessageSender;
        readonly LogMessageParser _logMessageParser;

        public TelegramLogNotifier(IOptions<Config> configuration)
        {
            _directoryWatcher = new DirectoryWatcher(configuration.Value.LogFilesDirectory, "*.log", ProcessFileChange);
            _telegramBotMessageSender = new TelegramBotMessageSender(configuration.Value.TelegramBotToken, configuration.Value.TelegramChatId);
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
