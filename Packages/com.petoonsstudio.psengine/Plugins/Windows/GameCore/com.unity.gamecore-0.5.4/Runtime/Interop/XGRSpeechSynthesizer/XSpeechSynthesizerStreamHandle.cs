using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //typedef struct XSpeechSynthesizerStream* XSpeechSynthesizerStreamHandle;
    [StructLayout(LayoutKind.Sequential)]
    internal struct XSpeechSynthesizerStreamHandle
    {
        internal readonly IntPtr Ptr;
    }
}
