using System;
using TelegramLogNotifier.Models;

namespace TelegramLogNotifier.interfaces
{
    public interface IFileEventNotifier : IDisposable
    {
        void Notify(FileEvent fileEvent);
    }
}
