namespace NEventStore.Persistence.GES
{
    public static class Headers
    {
        public static readonly string CommitId = "CommitId";
        public static readonly string CommitStamp = "CommitStamp";
        public static readonly string BucketId = "BucketId";
        public static readonly string CommitHeaders = "CommitHeaders";
        public static readonly string CommitSequence = "CommitSequence";
        public static readonly string StreamRevision = "StreamRevision";
    }
}