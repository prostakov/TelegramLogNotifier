using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TelegramLogNotifier.interfaces;
using TelegramLogNotifier.Models;
using TelegramLogNotifier.Notifiers;

namespace TelegramLogNotifier.DirectoryFileWatch
{
    public class FileWatcher : IDisposable
    {
        readonly int _fileSizeExceededThresholdBytes;
        readonly IFileEventNotifier _notifier;
        readonly CancellationTokenSource _cancelTokenSource;
        public string FilePath { get; private set; }
        int _lastAlertOccurenceHour = -1;

        public FileWatcher(IOptions<DirectoryFileWatchSettings> settings, IFileEventNotifier notifier)
        {
            _fileSizeExceededThresholdBytes = settings.Value.FileSizeExceededAlertThresholdBytes != 0
                ? settings.Value.FileSizeExceededAlertThresholdBytes
                : 104857600; // Set default to 100MB
            _notifier = notifier;

            _cancelTokenSource = new CancellationTokenSource();
            FilePath = string.Empty;
        }

        public void StartWatch(string filePath)
        {
            FilePath = filePath;

            var task = new Task(WatchFunc, _cancelTokenSource.Token, TaskCreationOptions.LongRunning);
            task.Start();

            Console.WriteLine($"Starting watching file: {FilePath}");
        }

        public void StopWatch()
        {
            if (!string.IsNullOrEmpty(FilePath))
            {
                Console.WriteLine($"Stopping watching file: {FilePath}");

                _cancelTokenSource.Cancel();

                FilePath = string.Empty;
            }
        }

        void WatchFunc()
        {
            using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    while (!sr.EndOfStream)
                    {
                        sr.ReadLine();
                    }

                    while (!_cancelTokenSource.IsCancellationRequested)
                    {
                        while (!sr.EndOfStream)
                        {
                            var fileEvent = new FileEvent { Type = FileEventType.Modified, Message = sr.ReadLine() };
                            _notifier.Notify(fileEvent);
                        }

                        while (sr.EndOfStream && !_cancelTokenSource.IsCancellationRequested)
                        {
                            Thread.Sleep(100);

                            if (sr.BaseStream.Length > _fileSizeExceededThresholdBytes)
                            {
                                AlertFileSizeExceeded();
                            }
                        }
                    }
                }
            }
        }

        void AlertFileSizeExceeded()
        {
            if (_lastAlertOccurenceHour == -1 || _lastAlertOccurenceHour != DateTime.UtcNow.Hour)
            {
                _lastAlertOccurenceHour = DateTime.UtcNow.Hour;

                var fileEvent = new FileEvent { Type = FileEventType.FileSizeExceeded, Message = FilePath };
                _notifier.Notify(fileEvent);
            }
        }

        public void Dispose()
        {
            StopWatch();
        }
    }
}
