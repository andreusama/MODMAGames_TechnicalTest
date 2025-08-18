using System;

namespace Unity.GameCore
{
    public class XGameSaveBlobInfo
    {
        public string Name { get; }
        public UInt32 Size { get; }

        internal XGameSaveBlobInfo(Interop.XGameSaveBlobInfo interopHandle)
        {
            Name = interopHandle.Name.GetString();
            Size = interopHandle.Size;
        }
    }
}
