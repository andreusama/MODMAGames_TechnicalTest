using System;

namespace Unity.GameCore
{
    public class XblMultiplayerSearchHandle
    {
        internal XblMultiplayerSearchHandle(Interop.XblMultiplayerSearchHandle interopHandle)
        {
            this.InteropHandle = interopHandle;
        }

        internal static Int32 WrapInteropHandleAndReturnHResult(Int32 hresult, Interop.XblMultiplayerSearchHandle interopHandle, out XblMultiplayerSearchHandle userHandle)
        {
            if (HR.SUCCEEDED(hresult))
            {
                userHandle = new XblMultiplayerSearchHandle(interopHandle);
            }
            else
            {
                userHandle = default(XblMultiplayerSearchHandle);
            }
            return hresult;
        }

        internal Interop.XblMultiplayerSearchHandle InteropHandle { get; }
    }
}
