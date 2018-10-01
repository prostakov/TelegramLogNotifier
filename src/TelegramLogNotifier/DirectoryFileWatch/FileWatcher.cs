using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TelegramLogNotifier.Models;
using TelegramLogNotifier.Notifiers;

namespace TelegramLogNotifier.DirectoryFileWatch
{
    public class FileWatcher : IDisposable
    {
        const int FileSizeExceededThresholdBytes = 104857600; // 100MB
        public readonly string FilePath;
        readonly IFileEventNotifier _notifier;
        readonly CancellationTokenSource _cancelTokenSource;
        int _lastAlertOccurenceHour = -1;

        public FileWatcher(string filePath, IFileEventNotifier notifier)
        {
            FilePath = filePath;
            _notifier = notifier;
            
            _cancelTokenSource = new CancellationTokenSource();
            var task = new Task(Watch, _cancelTokenSource.Token, TaskCreationOptions.LongRunning);
            task.Start();

            Console.WriteLine($"Starting watching file: {FilePath}");
        }

        void Watch()
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
                            var fileEvent = new FileEvent {Type = FileEventType.Modified, Message = sr.ReadLine()};
                            _notifier.Notify(fileEvent);
                        }

                        while (sr.EndOfStream && !_cancelTokenSource.IsCancellationRequested)
                        {
                            Thread.Sleep(100);

                            if (sr.BaseStream.Length > FileSizeExceededThresholdBytes)
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
                
                var fileEvent = new FileEvent {Type = FileEventType.FileSizeExceeded, Message = FilePath};
                _notifier.Notify(fileEvent);
            }
        }

        public void Dispose()
        {
            Console.WriteLine($"Stopping watching file: {FilePath}");
            _cancelTokenSource.Cancel();
        }
    }
}
