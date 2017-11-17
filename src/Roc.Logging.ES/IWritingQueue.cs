namespace Roc.Logging.ES
{
    public interface IWritingQueue
    {
         void Enqueue(EsLogEntity entity);
    }
}