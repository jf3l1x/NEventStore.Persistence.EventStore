namespace NEventStore.Persistence.EventStore.Projections
{
    public class ProjectionInfo
    {
        public string Status { get; set; }

        public bool IsEnabled
        {
            get { return Status.ToLower() == "running"; }
        }
    }
}