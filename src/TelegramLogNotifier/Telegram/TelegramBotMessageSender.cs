using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.Extensions.Options;
using TelegramLogNotifier.interfaces;

namespace TelegramLogNotifier.Telegram
{
    public class TelegramBotMessageSender : IMessageSender
    {
        readonly string _telegramBotSendMessageUrlTemplate;
        
        public TelegramBotMessageSender(IOptions<TelegramSettings> settings)
        {
            _telegramBotSendMessageUrlTemplate = $"https://api.telegram.org/bot{settings.Value.BotToken}/sendMessage?chat_id={settings.Value.ChatId}&parse_mode=HTML&text={"{0}"}";
        }

        public void Send(string message)
        {
            var url = string.Format(_telegramBotSendMessageUrlTemplate, message);

            var request = (HttpWebRequest)WebRequest.Create(url);

            var response = request.GetResponse();

            LogToConsole(response);
        }

        private void LogToConsole(WebResponse response)
        {
            var stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string text = reader.ReadToEnd();
            Console.WriteLine(text);
        }

        public void Dispose(){}
    }
}
