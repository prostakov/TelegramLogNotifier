using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TelegramLogNotifier
{
    public class FileChangeNotifier : IDisposable
    {
        readonly string _filePath;
        readonly Action<string> _processLine;
        readonly CancellationTokenSource _cancelTokenSource;

        public FileChangeNotifier(string filePath, Action<string> processLine)
        {
            _filePath = filePath;
            _processLine = processLine;
            _cancelTokenSource = new CancellationTokenSource();

            var task = new Task(Watch, _cancelTokenSource.Token, TaskCreationOptions.LongRunning);
            task.Start();
        }

        void Watch()
        {
            using (FileStream fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    while (!sr.EndOfStream) sr.ReadLine();

                    while (!_cancelTokenSource.IsCancellationRequested)
                    {
                        while (!sr.EndOfStream) _processLine(sr.ReadLine());
                        while (sr.EndOfStream) Thread.Sleep(100);
                    }
                }
            }
        }

        public void Dispose()
        {
            _cancelTokenSource.Cancel();
        }
    }
}
