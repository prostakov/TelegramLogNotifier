using System;
using TelegramLogNotifier.Models;

namespace TelegramLogNotifier.interfaces
{
    public interface IMessageSender : IDisposable
    {
        void Send(string message);
    }
}
