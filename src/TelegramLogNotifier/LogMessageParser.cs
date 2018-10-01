using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace TelegramLogNotifier
{
    public class LogMessageParser : IDisposable
    {
        private class SerilogLog
        {
            public DateTime Timestamp { get; set; }
            public string Level { get; set; }
            public string MessageTemplate { get; set; }
            public Dictionary<string, string> Properties { get; set; }
        }

        private class SerilogLogFormatter
        {
            public static string GetLogString(SerilogLog log)
            {
                var sb = new StringBuilder();

                sb.AppendLine($"<b>{log.Level}</b> - {log.Timestamp}");
                sb.AppendLine($"<code>{log.MessageTemplate}</code>");

                if (log.Properties.ContainsKey("SourceContext"))
                {
                    sb.AppendLine($"Context: {log.Properties["SourceContext"]}");
                }

                if (log.Properties.ContainsKey("RequestPath"))
                {
                    sb.AppendLine($"RequestPath: {log.Properties["RequestPath"]}");
                }

                return sb.ToString();
            }
        }

        public string Parse(string value)
        {
            if (TryParseLogObject<SerilogLog>(value, out SerilogLog log))
            {
                return SerilogLogFormatter.GetLogString(log);
            }

            return value;
        }

        bool TryParseLogObject<T>(string value, out T logObject)
        {
            var result = true;

            var settings = new JsonSerializerSettings
            {
                Error = (sender, args) => { result = false; args.ErrorContext.Handled = true; },
                MissingMemberHandling = MissingMemberHandling.Error
            };
            logObject = JsonConvert.DeserializeObject<T>(value, settings);

            Console.WriteLine(result);

            return result;
        }

        public void Dispose() { }
    }
}
