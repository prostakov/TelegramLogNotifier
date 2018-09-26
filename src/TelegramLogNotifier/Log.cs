// Sample
// {
//     "Timestamp":"2018-09-26T17:25:07.7986440+03:00",
//     "Level":"Fatal",
//     "MessageTemplate":"Testing FATAL logging...",
//     "Properties":
//     {
//         "SourceContext":"Web.Areas.Site.Controllers.TestController",
//         "ActionId":"1f27af75-8f95-45c1-a591-b1f30f8fef9d",
//         "ActionName":"Web.Areas.Site.Controllers.TestController.Error (Web)",
//         "RequestId":"0HLH344HV3U5G:0000000B",
//         "RequestPath":"/test/error",
//         "CorrelationId":null,
//         "ConnectionId":"0HLH344HV3U5G"
//     }
// }

using System;
using System.Collections.Generic;

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