namespace NEventStore.Persistence.EventStore.Models
{
    public class SnapshotMetadata
    {
        public SnapshotMetadata(Snapshot last)
        {
            if (last != null)
            {
                LastSnapshotStreamRevision = last.StreamRevision;
            }
        }

        public int LastSnapshotStreamRevision { get; set; }
    }
}