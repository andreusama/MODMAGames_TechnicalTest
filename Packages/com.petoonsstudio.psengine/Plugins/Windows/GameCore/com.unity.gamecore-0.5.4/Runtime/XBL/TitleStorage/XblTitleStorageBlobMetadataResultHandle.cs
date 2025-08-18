using System;

namespace Unity.GameCore
{
    public class XblTitleStorageBlobMetadataResultHandle
    {
        internal XblTitleStorageBlobMetadataResultHandle(Interop.XblTitleStorageBlobMetadataResultHandle interopHandle)
        {
            this.InteropHandle = interopHandle;
        }

        internal Interop.XblTitleStorageBlobMetadataResultHandle InteropHandle { get; set; }
    }
}
