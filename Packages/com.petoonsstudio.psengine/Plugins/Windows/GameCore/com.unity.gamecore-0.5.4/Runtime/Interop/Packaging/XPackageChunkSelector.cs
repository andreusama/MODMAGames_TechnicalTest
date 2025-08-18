using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //struct XPackageChunkSelector
    //{
    //    XPackageChunkSelectorType type;
    //    union
    //    {
    //        _Field_z_ const char* language;
    //        _Field_z_ const char* tag;
    //        uint32_t chunkId;
    //    };
    //};
    [StructLayout(LayoutKind.Explicit)]
    internal struct XPackageChunkSelector
    {
        [FieldOffset(0)]
        internal XPackageChunkSelectorType type;

        [FieldOffset(4)]
        internal UTF8StringPtr languageOrTag;

        [FieldOffset(4)]
        internal UInt32 chunkId;
    }
}
