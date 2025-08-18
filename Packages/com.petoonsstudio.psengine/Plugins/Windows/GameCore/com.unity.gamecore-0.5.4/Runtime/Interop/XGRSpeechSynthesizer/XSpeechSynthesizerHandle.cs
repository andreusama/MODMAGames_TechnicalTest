using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //typedef struct XSpeechSynthesizer* XSpeechSynthesizerHandle
    [StructLayout(LayoutKind.Sequential)]
    internal struct XSpeechSynthesizerHandle
    {
        internal readonly IntPtr Ptr;
    }
}
