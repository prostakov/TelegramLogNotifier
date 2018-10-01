using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TelegramLogNotifier.Models;
using TelegramLogNotifier.Notifiers;

namespace TelegramLogNotifier.DirectoryFileWatch
{
    public class DirectoryWatcher : IDisposable
    {
        readonly string _directoryPath;
        readonly string _directoryFileSearchPattern;
        readonly IFileEventNotifier _notifier;
        FileWatcher _currentFileWatcher;
        FileSystemWatcher _fileSystemWatcher;

        public DirectoryWatcher(IOptions<DirectoryFileWatchSettings> settings, IFileEventNotifier notifier)
        {
            _directoryPath = settings.Value.DirectoryPath;
            _directoryFileSearchPattern = settings.Value.FileSearchPattern;
            _notifier = notifier;

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

                    _currentFileWatcher = new FileWatcher(filePath, _notifier);
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
