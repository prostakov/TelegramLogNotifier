namespace TelegramLogNotifier.DirectoryFileWatch
{
    public class DirectoryFileWatchSettings
    {
        public string DirectoryPath { get; set; }
        public string FileSearchPattern { get; set; }
        public int FileSizeExceededAlertThresholdBytes { get; set; }
    }
}
