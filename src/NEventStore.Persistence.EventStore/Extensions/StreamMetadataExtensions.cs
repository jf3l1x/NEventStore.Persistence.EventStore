using EventStore.ClientAPI;

namespace NEventStore.Persistence.EventStore.Extensions
{
    public static class StreamMetadataExtensions
    {
        public static StreamMetadataBuilder Clone(this StreamMetadata metadata)
        {
            StreamMetadataBuilder retval =
                StreamMetadata.Build();
            if (metadata.Acl != null)
            {

                retval.SetMetadataReadRoles(metadata.Acl.MetaReadRoles)
                    .SetMetadataWriteRoles(metadata.Acl.MetaWriteRoles)
                    .SetDeleteRoles(metadata.Acl.DeleteRoles)
                    .SetWriteRoles(metadata.Acl.WriteRoles)
                    .SetReadRoles(metadata.Acl.ReadRoles);
            }
            if (metadata.CacheControl.HasValue)
            {
                retval.SetCacheControl(metadata.CacheControl.Value);
            }
            if (metadata.MaxAge.HasValue)
            {
                retval.SetMaxAge(metadata.MaxAge.Value);
            }
            if (metadata.MaxCount.HasValue)
            {
                retval.SetMaxCount(metadata.MaxCount.Value);
            }
            if (metadata.TruncateBefore.HasValue)
            {
                retval.SetTruncateBefore(metadata.TruncateBefore.Value);
            }
            foreach (string customKey in metadata.CustomKeys)
            {
                retval.SetCustomPropertyWithValueAsRawJsonString(customKey, metadata.GetValueAsRawJsonString(customKey));
            }
            return retval;
        }
    }
}