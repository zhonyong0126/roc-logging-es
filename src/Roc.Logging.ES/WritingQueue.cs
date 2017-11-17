using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Nest;

namespace Roc.Logging.ES
{
    class WritingQueue : IWritingQueue
    {
        private readonly ElasticClient esClient;
        private readonly BlockingCollection<EsLogEntity> queue;
        private readonly StreamWriter profiler = new StreamWriter("writing_queue_profiler.log", true, Encoding.UTF8) { AutoFlush = true };
        private readonly EsLoggerSettings settings;
        private readonly IDiagLogger diagLogger;

        public WritingQueue(EsLoggerSettings settings, IDiagLogger diagLogger)
        {
            this.diagLogger = diagLogger;
            this.settings = settings;
            var esSettings = new Nest.ConnectionSettings(new Uri(settings.EsUrl));
            esSettings.DefaultIndex(settings.IndexName);
            this.esClient = new Nest.ElasticClient(esSettings);

            this.queue = new BlockingCollection<EsLogEntity>();

            Task.Factory.StartNew(DoWrite, TaskCreationOptions.LongRunning);
        }

        private async Task DoWrite()
        {
            System.Diagnostics.Stopwatch watch = null;

            foreach (var item in this.queue.GetConsumingEnumerable())
            {
                if (null == watch && this.settings.EnableProfiler)
                {
                    watch = new System.Diagnostics.Stopwatch();
                }

                if (null != watch && !this.settings.EnableProfiler)
                {
                    watch = null;
                }

                watch?.Start();

                try
                {
                    var result = await this.esClient.IndexAsync(item);

                    if(result.ApiCall.ServerError!=null)
                    {
                        this.diagLogger.WriteLine(Microsoft.Extensions.Logging.LogLevel.Critical
                            ,$"Statue:{result.ApiCall.ServerError.Status} Reason:{result.ApiCall.ServerError.Error?.Reason}");
                    }
                }
                catch (System.Exception ex)
                {
                    this.diagLogger.WriteLine(Microsoft.Extensions.Logging.LogLevel.Critical, ex.Message);
                }

                watch?.Stop();
                if (null != watch)
                {
                    profiler.WriteLine(watch.ElapsedMilliseconds.ToString());
                }
            }
        }

        public void Enqueue(EsLogEntity entity)
        {
            this.queue.Add(entity);
        }
    }
}