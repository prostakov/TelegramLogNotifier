using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TelegramLogNotifier.interfaces;
using TelegramLogNotifier.Models;
using TelegramLogNotifier.Notifiers;

namespace TelegramLogNotifier.DirectoryFileWatch
{
    public class DirectoryWatcher : IDisposable
    {
        readonly string _directoryPath;
        readonly string _directoryFileSearchPattern;
        readonly FileWatcher _fileWatcher;
        FileSystemWatcher _fileSystemWatcher;

        public DirectoryWatcher(IOptions<DirectoryFileWatchSettings> settings, FileWatcher fileWatcher)
        {
            _directoryPath = settings.Value.DirectoryPath;
            _directoryFileSearchPattern = settings.Value.FileSearchPattern;
            _fileWatcher = fileWatcher;

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
                _fileWatcher.StopWatch();
            }
            else
            {
                if (filePath != _fileWatcher.FilePath)
                {
                    _fileWatcher.StopWatch();
                    _fileWatcher.StartWatch(filePath);
                }
            }
        }

        void FileSystemWatcher_Event(object sender, FileSystemEventArgs e)
        {
            SetFileWatcher();
        }

        public void Dispose()
        {
            if (_fileWatcher != null)
                _fileWatcher.Dispose();

            if (_fileSystemWatcher != null)
                _fileSystemWatcher.Dispose();
        }
    }
}
