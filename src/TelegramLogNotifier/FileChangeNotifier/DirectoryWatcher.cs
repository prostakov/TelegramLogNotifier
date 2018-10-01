using System;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TelegramLogNotifier.FileChangeNotifier
{
    public class DirectoryWatcher : IDisposable
    {
        readonly string _directoryPath;
        readonly string _directoryFileSearchPattern;
        readonly Action<FileEvent> _processFileEvent;
        FileWatcher _currentFileWatcher;
        FileSystemWatcher _fileSystemWatcher;

        public DirectoryWatcher(string directoryPath, string directoryFileSearchPattern, Action<FileEvent> processFileEvent)
        {
            _directoryPath = directoryPath;
            _directoryFileSearchPattern = directoryFileSearchPattern;
            _processFileEvent = processFileEvent;

            _fileSystemWatcher = new FileSystemWatcher();
            _fileSystemWatcher.Path = _directoryPath;
            _fileSystemWatcher.Created += FileSystemWatcher_Event;
            _fileSystemWatcher.Deleted += FileSystemWatcher_Event;
            _fileSystemWatcher.EnableRaisingEvents = true;

            Console.WriteLine($"Initialized directory watch on: {_directoryPath}");

            SetFileWatcher();
        }

        void SetFileWatcher()
        {
            var filePath = Directory.EnumerateFiles(_directoryPath, _directoryFileSearchPattern)
                .OrderByDescending(filename => filename)
                .FirstOrDefault();

            if (filePath == null)
            {
                if (_currentFileWatcher != null)
                    _currentFileWatcher.Dispose();
            }
            else
            {
                if (filePath != _currentFileWatcher?.FilePath)
                {
                    if (_currentFileWatcher != null)
                        _currentFileWatcher.Dispose();

                    _currentFileWatcher = new FileWatcher(filePath, _processFileEvent);
                }
            }
        }

        void FileSystemWatcher_Event(object sender, FileSystemEventArgs e)
        {
            SetFileWatcher();
        }

        public void Dispose()
        {
            if (_currentFileWatcher != null)
                _currentFileWatcher.Dispose();

            if (_fileSystemWatcher != null)
                _fileSystemWatcher.Dispose();
        }
    }
}
