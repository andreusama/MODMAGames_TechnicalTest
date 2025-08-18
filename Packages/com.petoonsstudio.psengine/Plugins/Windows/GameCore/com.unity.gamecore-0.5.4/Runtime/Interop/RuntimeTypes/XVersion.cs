using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    /*
        struct XVersion
        {
            uint16_t major;
            uint16_t minor;
            uint16_t build;
            uint16_t revision;
        };
    */

    [StructLayout(LayoutKind.Sequential)]
    internal struct XVersion
    {
        internal UInt16 major;
        internal UInt16 minor;
        internal UInt16 build;
        internal UInt16 revision;
    }
}
