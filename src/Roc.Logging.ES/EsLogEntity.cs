using System;
using Microsoft.Extensions.Logging;

namespace Roc.Logging.ES
{
    public class EsLogEntity
    {
        public string AppId { get; set; }
        public string CategoryName { get; set; }
        public int LogLevelId { get; set; }
        public string LogLevel { get; set; }
        public string EventId { get; set; }
        public string State { get; set; }
        public string Exception { get; set; }
        public string Message { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}