using System;

namespace Unity.GameCore
{
    public class XSpeechSynthesizerStreamHandle
    {
        internal XSpeechSynthesizerStreamHandle(Interop.XSpeechSynthesizerStreamHandle interopHandle)
        {
            this.InteropHandle = interopHandle;
        }

        internal static Int32 WrapInteropHandleAndReturnHResult(Int32 hresult, Interop.XSpeechSynthesizerStreamHandle interopHandle, out XSpeechSynthesizerStreamHandle speechSynthesisStream)
        {
            if (HR.SUCCEEDED(hresult))
            {
                speechSynthesisStream = new XSpeechSynthesizerStreamHandle(interopHandle);
            }
            else
            {
                speechSynthesisStream = default(XSpeechSynthesizerStreamHandle);
            }
            return hresult;
        }

        internal Interop.XSpeechSynthesizerStreamHandle InteropHandle { get; }
    }
}
