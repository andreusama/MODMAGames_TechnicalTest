using System;
using System.Collections.Generic;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XSpeechSynthesizerHandle
    {
        internal XSpeechSynthesizerHandle(Interop.XSpeechSynthesizerHandle interopHandle)
        {
            this.InteropHandle = interopHandle;
        }

        internal static Int32 WrapAndReturnHResult(Int32 hresult, Interop.XSpeechSynthesizerHandle interopHandle, out XSpeechSynthesizerHandle handle)
        {
            if (HR.SUCCEEDED(hresult))
            {
                handle = new XSpeechSynthesizerHandle(interopHandle);
            }
            else
            {
                handle = default(XSpeechSynthesizerHandle);
            }
            return hresult;
        }

        internal Interop.XSpeechSynthesizerHandle InteropHandle { get; private set; }
    }
}
