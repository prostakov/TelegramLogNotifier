using System;
using System.Collections.Generic;
using System.Net;

namespace TelegramLogNotifier
{
    public class TelegramBotMessageSender
    {
        readonly string _telegramBotSendMessageUrlTemplate;
        
        public TelegramBotMessageSender(string botToken, int chatId)
        {
            _telegramBotSendMessageUrlTemplate = $"https://api.telegram.org/bot{botToken}/sendMessage?chat_id={chatId}&parse_mode=HTML&text={"{0}"}";
        }

        public void SendMessage(string message)
        {
            var url = string.Format(_telegramBotSendMessageUrlTemplate, message);

            var request = (HttpWebRequest)WebRequest.Create(url);

            request.GetResponse();
        }
    }
}
