using System;
using Microsoft.Extensions.Configuration;
using Roc.Logging.ES;

namespace Microsoft.Extensions.Logging
{
    public static class EsLoggerFactoryExtensions
    {
        public static ILoggingBuilder AddElasticSearch(this ILoggingBuilder loggingBuilder,EsLoggerSettings settings,Func<LogLevel,bool> filter=null)
        {
            loggingBuilder.AddProvider(new EsLoggerProvider(settings,filter));
            return loggingBuilder;
        }
        public static ILoggingBuilder AddElasticSearch(this ILoggingBuilder loggingBuilder,IConfiguration configuration,Func<LogLevel,bool> filter=null)
        {
            return AddElasticSearch(loggingBuilder,new EsLoggerSettings(configuration));
        }

        public static ILoggerFactory AddElasticSearch(this ILoggerFactory loggerFactory,EsLoggerSettings settings,Func<LogLevel,bool> filter=null)
        {
            loggerFactory.AddProvider(new EsLoggerProvider(settings,filter));
            return loggerFactory;
        }
        public static ILoggerFactory AddElasticSearch(this ILoggerFactory loggerFactory,IConfiguration configuration,Func<LogLevel,bool> filter=null)
        {
            return AddElasticSearch(loggerFactory,new EsLoggerSettings(configuration));
        }
    }
}