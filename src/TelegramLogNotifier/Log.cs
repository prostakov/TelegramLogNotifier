using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramLogNotifier
{
	public class Log
    {
        public DateTime Timestamp { get; set; }
        public string Level { get; set; }
        public string MessageTemplate { get; set; }
        public Dictionary<string, string> Properties { get; set; }
    }
}
