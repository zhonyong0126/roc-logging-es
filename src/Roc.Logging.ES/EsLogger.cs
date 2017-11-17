using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Roc.Logging.ES
{
    internal class EsLogger : ILogger
    {
        private readonly EsLoggerSettings settings;
        private readonly Func<LogLevel, bool> filter;
        private readonly IDiagLogger diagnosticLog;
        private readonly IWritingQueue writingQueue;
        private readonly string categoryName;
        public EsLogger(string categoryName, EsLoggerSettings settings, Func<LogLevel, bool> filter, IDiagLogger diagnosticLog, IWritingQueue writingQueue)
        {
            this.categoryName = categoryName;
            this.settings = settings;
            this.filter = filter;
            this.diagnosticLog = diagnosticLog;
            this.writingQueue = writingQueue;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return NopDisposable.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            if (this.settings.Disabled)
            {
                return false;
            }

            if (null == this.filter)
            {
                return logLevel == LogLevel.Information;
            }

            return this.filter(logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (this.settings.Disabled)
            {
                return;
            }

            var entity = new EsLogEntity()
            {
                AppId = this.settings.AppId,
                CategoryName = this.categoryName,
                LogLevelId = (int)logLevel,
                LogLevel = logLevel.ToString(),
                EventId = $"{eventId.Id}({eventId.Name})",
                State = state == null ? (string)null : state.ToString(),
                Exception = exception == null ? (string)null : exception.ToString(),
                Message = formatter(state, exception),
                CreatedTime = DateTime.UtcNow,
            };
            this.writingQueue.Enqueue(entity);
        }

        private class NopDisposable : IDisposable
        {
            private NopDisposable()
            { }
            public static readonly IDisposable Instance = new NopDisposable();
            public void Dispose()
            {
            }
        }
    }
}
