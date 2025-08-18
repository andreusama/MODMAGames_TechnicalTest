using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unity.GameCore
{
    public class XblTitleStorageBlobMetadata
    {
        internal XblTitleStorageBlobMetadata(Interop.XblTitleStorageBlobMetadata interopHandle)
        {
            this.InteropHandle = interopHandle;

            this.BlobPath = interopHandle.GetBlobPath();
            this.BlobType = interopHandle.blobType;
            this.StorageType = interopHandle.storageType;
            this.DisplayName = interopHandle.GetDisplayName();
            this.ETag = interopHandle.GetETag();
            this.ClientTimestamp = interopHandle.clientTimestamp.DateTime;
            this.Length = interopHandle.length.ToUInt64();
            this.ServiceConfigurationId = interopHandle.GetServiceConfigurationId();
            this.XboxUserId = interopHandle.xboxUserId;
        }

        public string BlobPath { get; }
        public XblTitleStorageBlobType BlobType { get; }
        public XblTitleStorageType StorageType { get; }
        public string DisplayName { get; }
        public string ETag { get; }
        public DateTime ClientTimestamp { get; }
        public UInt64 Length { get; }
        public string ServiceConfigurationId { get; }
        public UInt64 XboxUserId { get; }

        internal Interop.XblTitleStorageBlobMetadata InteropHandle { get; }
    }
}
