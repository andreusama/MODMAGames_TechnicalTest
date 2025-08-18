using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XGameSaveContainerInfo
    {
        public string Name { get; }
        public string DisplayName { get; }
        public UInt32 BlobCount { get; }
        public UInt64 TotalSize { get; }
        public DateTime LastModifiedTime { get; }
        public bool NeedsSync { get; }

        internal XGameSaveContainerInfo(Interop.XGameSaveContainerInfo interopInfo)
        {
            Name = interopInfo.name.GetString();
            DisplayName = interopInfo.displayName.GetString();
            BlobCount = interopInfo.blobCount;
            TotalSize = interopInfo.totalSize;
            LastModifiedTime = interopInfo.lastModifiedTime.DateTime;
            NeedsSync = interopInfo.NeedsSync;
        }        
    }
}
