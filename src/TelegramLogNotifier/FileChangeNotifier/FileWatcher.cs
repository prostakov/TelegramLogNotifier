using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TelegramLogNotifier.FileChangeNotifier
{
    public class FileWatcher : IDisposable
    {
        const int FileSizeExceededThresholdBytes = 104857600; // 100MB
        public readonly string FilePath;
        readonly Action<FileEvent> _processFileEvent;
        readonly CancellationTokenSource _cancelTokenSource;
        int _lastAlertOccurenceHour = -1;

        public FileWatcher(string filePath, Action<FileEvent> processFileEvent)
        {
            FilePath = filePath;
            _processFileEvent = processFileEvent;
            
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
                            _processFileEvent(fileEvent);
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
                _processFileEvent(fileEvent);
            }
        }

        public void Dispose()
        {
            Console.WriteLine($"Stopping watching file: {FilePath}");
            _cancelTokenSource.Cancel();
        }
    }
}
