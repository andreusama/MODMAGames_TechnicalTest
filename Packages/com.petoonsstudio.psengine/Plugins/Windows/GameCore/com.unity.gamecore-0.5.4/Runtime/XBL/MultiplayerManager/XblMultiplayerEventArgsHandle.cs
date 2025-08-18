using System;

namespace Unity.GameCore
{
    public class XblMultiplayerEventArgsHandle
    {
        internal XblMultiplayerEventArgsHandle(Interop.XblMultiplayerEventArgsHandle interopHandle)
        {
            this.InteropHandle = interopHandle;
        }

        internal static Int32 WrapInteropHandleAndReturnHResult(Int32 hresult, Interop.XblMultiplayerEventArgsHandle interopHandle, out XblMultiplayerEventArgsHandle handle)
        {
            if (HR.SUCCEEDED(hresult))
            {
                handle = new XblMultiplayerEventArgsHandle(interopHandle);
            }
            else
            {
                handle = default(XblMultiplayerEventArgsHandle);
            }
            return hresult;
        }

        internal Interop.XblMultiplayerEventArgsHandle InteropHandle { get; set; }
    }
}
