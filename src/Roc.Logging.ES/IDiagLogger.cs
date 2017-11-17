using Microsoft.Extensions.Logging;

namespace Roc.Logging.ES
{
    internal interface IDiagLogger
    {
         void WriteLine(LogLevel logLevel, string message);
    }
}