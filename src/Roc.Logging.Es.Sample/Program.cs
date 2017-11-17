using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Roc.Logging.ES;

namespace Roc.Logging.Es.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var config=new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json",optional:true,reloadOnChange:true)
                .AddJsonFile("appSettings.Development.json",optional:true,reloadOnChange:true)
                .Build();

            var serviceProvider=new ServiceCollection()
                .AddLogging(builder=>{
                    builder.AddElasticSearch(config.GetSection("ElasticSearchLogging"));
                    builder.AddDebug();
                    builder.AddConfiguration(config.GetSection("Logging"));
                })
                .BuildServiceProvider();
            
            var logger=serviceProvider.GetRequiredService<ILogger<Program>>();
            var checkNameLogger=serviceProvider.GetRequiredService<ILogger<CheckName>>();

             //logger.IsEnabled(LogLevel.Information);
            
            // logger.LogCritical("Test Critical");
            try
            {
                ThrowNotImpleException();
            }
            catch (System.Exception ex)
            {
                logger.LogError(new EventId(2,"Error"),ex,"调用方法时出现异常");
            }
            //logger.LogCritical("Test Critical log");
            // while(true)
            // {
            //     Console.WriteLine(":");
            //     var input=Console.ReadLine();
            // }

            Console.ReadLine();
        }

        static void ThrowNotImpleException()
        {
            throw new NotImplementedException("该方法未实现");
        }
    }

    class CheckName
    {
        
    }
}
