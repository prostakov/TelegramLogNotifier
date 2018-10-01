using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Options;

namespace TelegramLogNotifier.Telegram
{
    public class TelegramBotMessageSender : IDisposable
    {
        readonly string _telegramBotSendMessageUrlTemplate;
        
        public TelegramBotMessageSender(IOptions<TelegramSettings> settings)
        {
            _telegramBotSendMessageUrlTemplate = $"https://api.telegram.org/bot{settings.Value.BotToken}/sendMessage?chat_id={settings.Value.ChatId}&parse_mode=HTML&text={"{0}"}";
        }

        public void SendMessage(string message)
        {
            var url = string.Format(_telegramBotSendMessageUrlTemplate, message);

            var request = (HttpWebRequest)WebRequest.Create(url);

            request.GetResponse();
        }

        public void Dispose(){}
    }
}
