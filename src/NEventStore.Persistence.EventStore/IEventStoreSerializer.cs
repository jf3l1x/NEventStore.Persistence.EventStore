namespace NEventStore.Persistence.GES
{
    public interface IEventStoreSerializer
    {
        bool IsJsonSerializer { get; }
        byte[] Serialize(object graph);
        object Deserialize(string type, byte[] data);
    }
}