namespace NEventStore.Persistence.EventStore.Services
{
    public interface IEventStoreSerializer
    {
        bool IsJsonSerializer { get; }
        byte[] Serialize(object graph);
        object Deserialize(string type, byte[] data);
        T Deserialize<T>(byte[] data);
    }
}