using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Roc.Logging.ES
{
    internal class DiagLogger : IDiagLogger, IDisposable
    {
        private readonly StreamWriter writer;

        public DiagLogger(string filePath)
        {
            this.writer = new StreamWriter(filePath, true, encoding: Encoding.UTF8)
            {
                AutoFlush = true
            };
        }

        public void Dispose()
        {
            this.writer.Close();
            this.writer.Dispose();
        }

        public void WriteLine(LogLevel logLevel, string message)
        {
            this.writer.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ttt")} {logLevel} {message}");
        }
    }
}