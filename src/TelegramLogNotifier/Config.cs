namespace TelegramLogNotifier
{
    public class Config
    {
        public string LogFilesDirectory { get; set; }
        public string TelegramBotToken { get; set; }
        public int TelegramChatId { get; set; }
    }
}
