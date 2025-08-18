using System;

namespace Unity.GameCore
{
    public class XblMultiplayerSessionHandle
    {
        internal XblMultiplayerSessionHandle(Interop.XblMultiplayerSessionHandle interopHandle)
        {
            this.InteropHandle = interopHandle;
        }

        internal static Int32 WrapInteropHandleAndReturnHResult(Int32 hresult, Interop.XblMultiplayerSessionHandle interopHandle, out XblMultiplayerSessionHandle sessionHandle)
        {
            if (HR.SUCCEEDED(hresult))
            {
                sessionHandle = new XblMultiplayerSessionHandle(interopHandle);
            }
            else
            {
                sessionHandle = default(XblMultiplayerSessionHandle);
            }
            return hresult;
        }

        internal Interop.XblMultiplayerSessionHandle InteropHandle { get; }
    }
}