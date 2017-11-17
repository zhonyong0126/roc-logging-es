using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Roc.Logging.ES
{
    [ProviderAlias("EsLogger")]
    public class EsLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, EsLogger> loggerCache = new ConcurrentDictionary<string, EsLogger>();
        private readonly EsLoggerSettings settings;
        private readonly Func<LogLevel, bool> filter;
        private readonly DiagLogger esLoggerDiagLog;
        private readonly DiagLogger writingQueueDiagLog;

        public EsLoggerProvider(EsLoggerSettings settings, Func<LogLevel, bool> filter)
        {
            this.settings = settings;
            this.filter = filter;
            this.esLoggerDiagLog=new DiagLogger("EsLogger_Diag.log");
            this.writingQueueDiagLog=new DiagLogger("EsLogger_WritingQueue_Diag.log");
        }

        public ILogger CreateLogger(string categoryName)
        {
            return this.loggerCache.GetOrAdd(categoryName, CreateEsLogger);
        }

        private EsLogger CreateEsLogger(string categoryName)
        {
            return new EsLogger(categoryName, this.settings, this.filter, this.esLoggerDiagLog,new WritingQueue(this.settings,this.writingQueueDiagLog));
        }

        public void Dispose()
        {
            loggerCache.Clear();
            this.esLoggerDiagLog.Dispose();
            this.writingQueueDiagLog.Dispose();
        }
    }
}