using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TelegramLogNotifier
{
    public class FileChangeNotifier : IDisposable
    {
        readonly string _filePath;
        readonly Action<string> _processFileChangeDelegate;
        readonly Action _alertFileSizeExceededDelegate;
        readonly int _fileSizeExceededThresholdBytes = 104_857_600; // 100MB
        readonly CancellationTokenSource _cancelTokenSource;
        DateTime? _lastAlertDate;

        public FileChangeNotifier(string filePath, Action<string> processFileChangeDelegate, Action alertFileSizeExceededDelegate)
        {
            _filePath = filePath;
            _processFileChangeDelegate = processFileChangeDelegate;
            _alertFileSizeExceededDelegate = alertFileSizeExceededDelegate;
            _cancelTokenSource = new CancellationTokenSource();

            var task = new Task(Watch, _cancelTokenSource.Token, TaskCreationOptions.LongRunning);
            task.Start();
        }

        public FileChangeNotifier(string filePath, Action<string> processFileChangeDelegate, Action alertFileSizeExceededDelegate, int fileSizeExceededThresholdBytes)
         : this(filePath, processFileChangeDelegate, alertFileSizeExceededDelegate)
        {
            _fileSizeExceededThresholdBytes = fileSizeExceededThresholdBytes;
        }

        void Watch()
        {
            using (FileStream fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
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
                            _processFileChangeDelegate(sr.ReadLine());
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
            if (!_lastAlertDate.HasValue || _lastAlertDate.Value.DayOfYear != DateTime.UtcNow.DayOfYear)
            {
                _lastAlertDate = DateTime.UtcNow;
                _alertFileSizeExceededDelegate();
            }
        }

        public void Dispose()
        {
            _cancelTokenSource.Cancel();
        }
    }
}
