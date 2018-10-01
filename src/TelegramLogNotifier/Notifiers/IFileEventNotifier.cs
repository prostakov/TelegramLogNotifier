using System;
using TelegramLogNotifier.Models;

namespace TelegramLogNotifier.Notifiers
{
    public interface IFileEventNotifier : IDisposable
    {
        void Notify(FileEvent fileEvent);
    }
}
