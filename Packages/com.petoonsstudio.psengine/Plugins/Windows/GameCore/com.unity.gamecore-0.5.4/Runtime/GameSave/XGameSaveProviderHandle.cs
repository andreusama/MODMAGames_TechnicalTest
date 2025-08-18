using System;

namespace Unity.GameCore
{
    public class XGameSaveProviderHandle
    {
        internal XGameSaveProviderHandle(Interop.XGameSaveProviderHandle interopHandle)
        {
            this.InteropHandle = interopHandle;
        }

        internal static Int32 WrapInteropHandleAndReturnHResult(Int32 hresult, Interop.XGameSaveProviderHandle interopHandle, out XGameSaveProviderHandle userHandle)
        {
            if (HR.SUCCEEDED(hresult))
            {
                userHandle = new XGameSaveProviderHandle(interopHandle);
            }
            else
            {
                userHandle = default(XGameSaveProviderHandle);
            }
            return hresult;
        }

        internal Interop.XGameSaveProviderHandle InteropHandle { get; }
    }
}
