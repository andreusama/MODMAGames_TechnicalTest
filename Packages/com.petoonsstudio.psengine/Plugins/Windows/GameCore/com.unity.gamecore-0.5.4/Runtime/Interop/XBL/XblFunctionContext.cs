using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //typedef int32_t XblFunctionContext;
    [StructLayout(LayoutKind.Sequential)]
    internal struct XblFunctionContext
    {
        private readonly Int32 context;
    }
}
